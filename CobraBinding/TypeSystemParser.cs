using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.TypeSystem;

using Cobra.Compiler;

namespace MonoDevelop.Cobra
{

	// TODO: Using the Cobra visitor may be too slow for this application
	public class TypeSystemParser : 
		global::Cobra.Core.Visitor,
		MonoDevelop.Ide.TypeSystem.ITypeSystemParser
	{
		private OptionValues _options;
		private List<ICSharpCode.NRefactory.TypeSystem.Error> _errors;
		private List<MonoDevelop.Ide.TypeSystem.Tag> _comments;
		private List<MonoDevelop.Ide.TypeSystem.FoldingRegion> _folds;
		private List<MonoDevelop.Ide.TypeSystem.ConditionalRegion> _conditionals;
		private string _content;
		private Dictionary<int, string> _lines;
		
		public TypeSystemParser()
		{
			_options = new OptionValues();
			_options.Add("compile", "compile");
			_options.Add("back-end", "clr");
			_options.Add("ert", "no");
			_options.Add("number", "decimal");
		}
		
		public override string MethodName {
			get { return "Visit"; }
		}

		//TODO: Clean up this mess
		public MonoDevelop.Ide.TypeSystem.ParsedDocument Parse(bool storeAst, string fileName, System.IO.TextReader content, MonoDevelop.Projects.Project project = null)
		{
			_content = content.ReadToEnd();
			var reader = new System.IO.StringReader(_content);
			_lines = new Dictionary<int, string>();

			int i = 1;
			string line = null;
			do {
				line = reader.ReadLine();
				if (line != null) {
					_lines[i] = line;
				}
				i++;
			} while (line != null);
			
			var parsedDoc = new MonoDevelop.Ide.TypeSystem.DefaultParsedDocument(fileName);

			//The parser needs a valid compiler and backend
			var c = new Compiler(0); //verbosity = 0
			c.Options = _options;
			c.InitBackEnd();
			
			//If we don't set these then we can't resolve all types
			Node.SetCompiler(c);
			Node.TypeProvider = c;
			
			//setup the parser
			var parser = new CobraParser();
			parser.TypeProvider = c;
			parser.WarningRecorder = c;
			parser.ErrorRecorder = c;
			parser.GlobalNS = c.GlobalNS;
			parser.BackEnd = c.BackEnd;

			CobraModule module = null;

			//parse the code
			try {
				module = parser.ParseSource(fileName, _content);
			} catch (Exception e) {
				if (c.Errors.Count != 0) {
					//hmm...why wasn't this reported to the error recorder?
					var se = new SourceException(e.Message);
					c.Errors.Add(se);
				}
			}		
			
			
			if (module != null) {		
				try {
					c.CurModule = module;			
					Box.SetCompiler(c);
					/*
					new BindUsePhase(c).Run();
					new BindInheritancePhase(c).Run();
					new BindInterfacePhase(c).Run();
					new ComputeMatchingBaseMembersPhase(c).Run();
					new BindImplementationPhase(c).Run();
					new IdentifyMainPhase(c).Run();
					new GenerateSharpCodePhase(c).Run();
					new CompileSharpCodePhase(c).Run();
					*/
				} catch (Exception e) {
					//hmmm...what should we do with this?
					MonoDevelop.Core.LoggingService.LogDebug(e.Message);
				}			
			}
			
			_errors = new List<Error>();
			foreach (var parseError in c.Errors) {
				var err = new ICSharpCode.NRefactory.TypeSystem.Error(
					ICSharpCode.NRefactory.TypeSystem.ErrorType.Error, parseError.Message, parseError.LineNum, 1);
				_errors.Add(err);
			}			
			
			foreach (var w in c.Warnings) {
				var warning = new ICSharpCode.NRefactory.TypeSystem.Error(
					ICSharpCode.NRefactory.TypeSystem.ErrorType.Warning, w.Message, w.LineNum, 1);
				_errors.Add(warning);
			}

			if (storeAst) {
				parsedDoc.Ast = module;
			}
			
			_comments = new List<MonoDevelop.Ide.TypeSystem.Tag>();
			_conditionals = new List<MonoDevelop.Ide.TypeSystem.ConditionalRegion>();
			_folds = new List<MonoDevelop.Ide.TypeSystem.FoldingRegion>();
			
			if (c.Errors.Count == 0 && module != null) {
				foreach (var dec in module.TopNameSpace.DeclsInOrder) {
					this.Dispatch(dec);
				}
			}
			
			parsedDoc.Add(_errors);
			parsedDoc.Add(_comments);
			parsedDoc.Add(_conditionals);
			parsedDoc.Add(_folds);

			return parsedDoc;
		}
		
		private int _GetIndentLevel(int row)
		{
			
			int level = 0;
			
			try {
				string line = _lines[row];
				
				foreach (var c in line) {
					if (c == '\t') {
						level++;
					} else {
						return level;
					}
				}
				
			} catch (Exception e) {
				Console.WriteLine("Exception in TypeSystemParser._GetIndentLevel" + e.Message);
				return level;
			}

			return level;
		}

		/*
		 * Returns the row number of the last row that has the same
		 * indentation level as the given starting row.
		 */
		private int _GetLastRow(int firstRow)
		{
		
			int currentIndentLevel = _GetIndentLevel(firstRow);
			var lastRow = firstRow;
			
			for (var row = firstRow + 1; row <= _lines.Count; row++) {

				if (_lines[row] != null && _lines[row].TrimStart().Length != 0) {
					//this is not a blank line

					if (_GetIndentLevel(row) <= currentIndentLevel) {
						//this line is at the same or previous indent level
						return lastRow;
					}

					lastRow = row;
				}
			}
			
			return lastRow;
		}

		private void _AddRegion(int firstRow)
		{
			int lastRow = _GetLastRow(firstRow);					
			int firstCol = _lines[firstRow].Length + 1; //add 1 so we don't hide the last character of the class name
			int lastCol = _lines[lastRow].TrimEnd().Length;

			if (firstRow == lastRow) {
				return;
			}

			var foldType = MonoDevelop.Ide.TypeSystem.FoldType.Undefined;
			var domRegion = new ICSharpCode.NRefactory.TypeSystem.DomRegion(
				firstRow, firstCol, lastRow, lastCol);
			
			var fold = new MonoDevelop.Ide.TypeSystem.FoldingRegion("...", domRegion, foldType, false);
			_folds.Add(fold);
		}

		public void Visit(NameSpace ns)
		{
			if (ns.Name != "global") {
				//TODO: Handle case when code under namespace is not indented
				_AddRegion(ns.Token.LineNum);
			}

			foreach (var dec in ns.DeclsInOrder) {
				this.Dispatch(dec);
			}
		}

		public void Visit(EnumDecl e)
		{
			_AddRegion(e.Token.LineNum);
		}
		
		public void Visit(Class c)
		{
			foreach (IBoxMember m in c.AllMembers()) {
				this.Dispatch(m);
			}

			foreach (var t in c.TestMethods) {
				this.Dispatch(t);
			}

			_AddRegion(c.Token.LineNum);
		}

		public void Visit(Initializer i)
		{
			_AddRegion(i.Token.LineNum);
		}

		public void Visit(TestMethod t)
		{
			_AddRegion(t.Token.LineNum);
		}

		public void Visit(Method m)
		{
			_AddRegion(m.Token.LineNum);
		}

		public void Visit(Field f)
		{
			return;
		}

		public void Visit(BoxVar v)
		{
			return;
		}

		public void Visit(Property p)
		{
			_AddRegion(p.Token.LineNum);
		}

		public void Visit(PropertyGetter g)
		{
			_AddRegion(g.Token.LineNum);
		}

		public void Visit(PropertySetter s)
		{
			_AddRegion(s.Token.LineNum);
		}
	}
}
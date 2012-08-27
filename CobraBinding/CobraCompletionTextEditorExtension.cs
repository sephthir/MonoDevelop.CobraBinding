using System;
using System.Collections.Generic;

namespace MonoDevelop.Cobra
{
	//Let's use fully-qualified names until we are familiar with these libraries...
	public class CobraCompletionTextEditorExtension : 
		MonoDevelop.Ide.Gui.Content.CompletionTextEditorExtension,
		MonoDevelop.Ide.Gui.Content.ITextEditorMemberPositionProvider
	{
		List<System.Reflection.Assembly> _implicitRefs;
		List<string> _implicitNamespaces;
		MonoDevelop.Ide.CodeCompletion.CompletionDataList _implicitComplData;
		MonoDevelop.Ide.CodeCompletion.CompletionDataList _keywords;
		
		public CobraCompletionTextEditorExtension() : base()
		{
			//these are implicitly "use"d in Cobra
			_implicitNamespaces = new List<string>();
			_implicitNamespaces.Add("System");
			_implicitNamespaces.Add("System.Collections.Generic");
			_implicitNamespaces.Add("System.IO");
			_implicitNamespaces.Add("System.Text");
			_implicitNamespaces.Add("Cobra.Core");
			
			//these assemblies provide the implicitly used namespaces
			_implicitRefs = new List<System.Reflection.Assembly>();
			_implicitRefs.Add(System.Reflection.Assembly.Load("mscorlib"));
			//_implicitRefs.Add(System.Reflection.Assembly.Load("System"));
			_implicitRefs.Add(System.Reflection.Assembly.Load("Cobra.Core"));
			
			_implicitComplData = new MonoDevelop.Ide.CodeCompletion.CompletionDataList();
			foreach (var asm in _implicitRefs) {
				foreach (var t in asm.GetTypes()) {
					
					if (t.IsPublic && _implicitNamespaces.Contains(t.Namespace)) {
						
						MonoDevelop.Core.IconId ico = null;
	
						if (t.IsClass) {
							//TODO differentiate between classes and delegates
							ico = new MonoDevelop.Core.IconId("md-class");
						} else if (t.IsInterface) {
							ico = new MonoDevelop.Core.IconId("md-interface");
						} else if (t.IsEnum) {
							ico = new MonoDevelop.Core.IconId("md-enum");
						} else {
							//ico = new MonoDevelop.Core.IconId("md-struct");
						}
						
						string name;
						
						if (t.IsGenericType) {
							string[] parts = t.Name.Split("`".ToCharArray());
							
							int count = int.Parse(parts[1]);
							if (count == 1) {
								name = parts[0] + "<of T>";
							} else {
								name = parts[0] + "<of ";
								for (var i = 0; i < count; i++) {
									if (i > 0) {
										name += ", ";
									}
									name += "T" + (i + 1).ToString();
								}
								name += ">";
							}
							
						} else {
							name = t.Name;
						}
						
						_implicitComplData.Add(new MonoDevelop.Ide.CodeCompletion.CompletionData(
							name, ico, t.FullName.Replace(t.Name, name))
						);
					}
				}
			}

			//all Cobra keywords
			_keywords = new MonoDevelop.Ide.CodeCompletion.CompletionDataList();
			foreach (var kw in global::Cobra.Compiler.KeywordSpecs.Keywords()) {
				_keywords.Add(kw, new MonoDevelop.Core.IconId("md-keyword"));
			}
		}
		
		//TODO
		public override bool KeyPress(Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{	
			bool result = base.KeyPress(key, keyChar, modifier);
			return result;
		}
		
		//TODO
		public override MonoDevelop.Ide.CodeCompletion.ICompletionDataList HandleCodeCompletion(
			MonoDevelop.Ide.CodeCompletion.CodeCompletionContext completionContext,
			char completionChar, ref int triggerWordLength)
		{
			if (completionChar == '.') {
				//TODO return fields and methods
				return null;
			}
			
			if (completionChar != ' ') {
				//do this so that the letter just typed is considered part of the word to complete
				triggerWordLength += 1;
			}
			
			var dataList = new MonoDevelop.Ide.CodeCompletion.CompletionDataList();
			
			dataList.AddRange(_implicitComplData);
			dataList.AddRange(_keywords);
			
			return dataList;
		}
		
		
		//TODO
		public override MonoDevelop.Ide.CodeCompletion.ICompletionDataList CodeCompletionCommand(
			MonoDevelop.Ide.CodeCompletion.CodeCompletionContext completionContext)
		{
			var dataList = new MonoDevelop.Ide.CodeCompletion.CompletionDataList();

			dataList.Add(this.GetTypeAt(completionContext.TriggerOffset).Name);
			//dataList.AddRange(_implicitComplData);
			//dataList.AddRange(_keywords);
			
			return dataList;
		}
		
		//TODO
		public override ICSharpCode.NRefactory.Completion.IParameterDataProvider HandleParameterCompletion(MonoDevelop.Ide.CodeCompletion.CodeCompletionContext completionContext, char completionChar)
		{
			return base.HandleParameterCompletion(completionContext, completionChar);
		}
		
		//TODO
		public override bool GetParameterCompletionCommandOffset(out int cpos)
		{
			return base.GetParameterCompletionCommandOffset(out cpos);
		}
		
		//TODO
		public override int GetCurrentParameterIndex(int startOffset)
		{
			return base.GetCurrentParameterIndex(startOffset);
		}

				
		//TODO
		public ICSharpCode.NRefactory.TypeSystem.IUnresolvedTypeDefinition GetTypeAt(int offset)
		{
			Console.WriteLine("GetTypeAt " + offset.ToString());
			return (ICSharpCode.NRefactory.TypeSystem.Implementation.DefaultUnresolvedTypeDefinition)
				document.ParsedDocument.Ast;
		}
		
		//TODO
		public ICSharpCode.NRefactory.TypeSystem.IUnresolvedMember GetMemberAt(int offset)
		{
			Console.WriteLine("GetMemberAt " + offset.ToString());
			return null;
		}
	}
}
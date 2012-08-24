using System;
using System.Collections.Generic;

namespace MonoDevelop.Cobra
{
	public class CobraParsedFile : ICSharpCode.NRefactory.TypeSystem.IParsedFile
	{
		public CobraParsedFile()
		{
		}

		//
		// Properties
		//

		public IList<ICSharpCode.NRefactory.TypeSystem.IUnresolvedAttribute> AssemblyAttributes {
			get { return null;}
		}

		public IList<ICSharpCode.NRefactory.TypeSystem.Error> Errors {
			get { return null;}
		}

		public string FileName {
			get { return "TODO";}
		}

		public DateTime? LastWriteTime {
			get;
			set;
		}

		public IList<ICSharpCode.NRefactory.TypeSystem.IUnresolvedAttribute> ModuleAttributes {
			get { return null;}
		}

		public IList<ICSharpCode.NRefactory.TypeSystem.IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return null;}
		}

		//
		// Methods
		//

		public ICSharpCode.NRefactory.TypeSystem.IUnresolvedTypeDefinition GetInnermostTypeDefinition(ICSharpCode.NRefactory.TextLocation location)
		{
			return null;
		}

		public ICSharpCode.NRefactory.TypeSystem.IUnresolvedMember GetMember(ICSharpCode.NRefactory.TextLocation location)
		{
			return null;
		}

		public ICSharpCode.NRefactory.TypeSystem.IUnresolvedTypeDefinition GetTopLevelTypeDefinition(ICSharpCode.NRefactory.TextLocation location)
		{
			return null;
		}

		public ICSharpCode.NRefactory.TypeSystem.ITypeResolveContext GetTypeResolveContext(ICSharpCode.NRefactory.TypeSystem.ICompilation compilation, ICSharpCode.NRefactory.TextLocation loc)
		{
			return null;
		}
	}
}


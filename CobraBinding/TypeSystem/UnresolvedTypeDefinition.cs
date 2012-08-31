using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.CobraBinding
{
	public class UnresolvedClass : ICSharpCode.NRefactory.TypeSystem.Implementation.DefaultUnresolvedTypeDefinition
	{
		Cobra.Compiler.Class _class;

		public UnresolvedClass(Cobra.Compiler.Class c) : base(c.ParentNameSpace.Name, c.Name)
		{
			_class = c;
		}

		public Cobra.Compiler.Class Class {
			get { return _class; }
		}
	}
}


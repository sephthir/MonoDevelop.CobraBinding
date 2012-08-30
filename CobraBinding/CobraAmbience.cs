using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.CobraBinding
{
	public class CobraAmbience : MonoDevelop.Ide.TypeSystem.Ambience
	{
		Dictionary<string, string> _dataTypes;
		Dictionary<ICSharpCode.NRefactory.TypeSystem.TypeKind, string> _classTypes;

		public CobraAmbience() : base(CobraLanguageBinding.LanguageName)
		{
			_dataTypes = new Dictionary<string, string>();
			_dataTypes["Void"] = ""; // is this valid?
			_dataTypes["Object"] = "Object";
			_dataTypes["Boolean"] = "bool";
			_dataTypes["Char"] = "char";
			_dataTypes["String"] = "String";
			_dataTypes["Int32"] = "int";
			_dataTypes["UInt32"] = "uint";
			_dataTypes["Double"] = "float";
			_dataTypes["Decimal"] = "decimal";
			_dataTypes["SByte"] = "int8";
			_dataTypes["Byte"] = "uint8";
			_dataTypes["Int16"] = "int16";
			_dataTypes["UInt16"] = "uint16";
			_dataTypes["Int64"] = "int64";
			_dataTypes["UInt64"] = "uint64";
			_dataTypes["Single"] = "float32";
			_dataTypes["Enum"] = "enum";

			_classTypes = new Dictionary<TypeKind, string>();
			_classTypes[TypeKind.Class] = "class";
			_classTypes[TypeKind.Enum] = "enum";
			_classTypes[TypeKind.Interface] = "interface";
			_classTypes[TypeKind.Struct] = "struct";
			_classTypes[TypeKind.Delegate] = "sig";
			//TODO: the rest?
		}

		public override string SingleLineComment(string text)
		{
			return CobraLanguageBinding.CommentTag + text;
		}

		public override string GetIntrinsicTypeName(string reflectionName)
		{
			var key = reflectionName.Replace("System.", String.Empty);
			return _dataTypes.ContainsKey(key) ? _dataTypes[key] : reflectionName;
		}

		public override string GetString(string nameSpace, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			var result = new System.Text.StringBuilder();

			if (settings.IncludeKeywords)
				result.Append(settings.EmitKeyword("namespace"));

			result.Append(Format(nameSpace));
			return result.ToString();
		}

		#region TODO

		protected override string GetTypeString(ICSharpCode.NRefactory.TypeSystem.IType type, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			if (type.Kind == TypeKind.Unknown) {
				return type.Name; //TODO: should this be dynamic?
			}

			if (type.Kind == TypeKind.TypeParameter) {
				return type.FullName;
			}

			//TODO: TypeWithElementType

			var typeDef = type.GetDefinition();
			if (typeDef == null) {
				return String.Empty;
			}

			if (!settings.UseNETTypeNames &&
				type.TypeParameterCount == 0 &&
				type.Namespace == "System" &&
				_dataTypes.ContainsKey(type.Name)) {

				return _dataTypes[type.Name];
			}

			return "WTF";
			//TODO: Figure out what I just wrote
		}

		protected override string GetMethodString(ICSharpCode.NRefactory.TypeSystem.IMethod method, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetTypeReferenceString(ICSharpCode.NRefactory.TypeSystem.IType reference, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetConstructorString(ICSharpCode.NRefactory.TypeSystem.IMethod constructor, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetDestructorString(ICSharpCode.NRefactory.TypeSystem.IMethod destructor, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetOperatorString(ICSharpCode.NRefactory.TypeSystem.IMethod op, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetFieldString(ICSharpCode.NRefactory.TypeSystem.IField field, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetEventString(ICSharpCode.NRefactory.TypeSystem.IEvent evt, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetPropertyString(ICSharpCode.NRefactory.TypeSystem.IProperty property, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetIndexerString(ICSharpCode.NRefactory.TypeSystem.IProperty property, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}

		protected override string GetParameterString(ICSharpCode.NRefactory.TypeSystem.IParameterizedMember member, ICSharpCode.NRefactory.TypeSystem.IParameter parameter, MonoDevelop.Ide.TypeSystem.OutputSettings settings)
		{
			throw new System.NotImplementedException();
		}
		#endregion

	}
}
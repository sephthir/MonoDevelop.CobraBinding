using System;

namespace MonoDevelop.CobraBinding
{
	public class CobraFormattingTextEditorExtension : MonoDevelop.Ide.Gui.Content.TextEditorExtension
	{
		public CobraFormattingTextEditorExtension() : base()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override bool KeyPress(Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			if (key == Gdk.Key.Return || key == Gdk.Key.KP_Enter) {
				//string lineIndent = this.Editor.GetLineIndent(this.Editor.Caret.Line - 1);
				//Console.WriteLine("'" + lineIndent + "'");
				//Console.WriteLine(lineIndent.Length);
			}
			return base.KeyPress(key, keyChar, modifier);
		}


	}
}
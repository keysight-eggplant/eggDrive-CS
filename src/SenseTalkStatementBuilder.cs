using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TestPlant.EggDriver
{
	public enum SenseTalkStatementType { Expression, Command, Function }

	public class SenseTalkStatementBuilder
	{
		protected const string DefaultCommandName = "Put";

		public SenseTalkStatementType StatementType = SenseTalkStatementType.Command;
		public object NameOrExpression = DefaultCommandName;
		public LinkedList<object> Parameters = new LinkedList<object>();
		public Dictionary<string, object> PropertyListParameters = new Dictionary<string, object>();

		public SenseTalkStatementBuilder () {}

		public SenseTalkStatementBuilder (string commandName) 
		{
			this.NameOrExpression = commandName;
		}

		public void Reset ()
		{
			StatementType = SenseTalkStatementType.Command;
			NameOrExpression = DefaultCommandName;
			Parameters = new LinkedList<object> ();
			PropertyListParameters = new Dictionary<string, object> ();
		}

		public SenseTalkStatementBuilder (object nameOrExpression, SenseTalkStatementType statementType) 
		{
			this.NameOrExpression = nameOrExpression;
			this.StatementType = statementType;
		}

		public SenseTalkStatementBuilder AddParameter (object parameter)
		{
			if (parameter != null)
				this.Parameters.AddLast (parameter);
			return this;
		}

		public SenseTalkStatementBuilder AddParameters (object[] parameters)
		{
			if (parameters != null) foreach (object parameter in parameters)
				AddParameter (parameter);
			return this;
		}

		public SenseTalkStatementBuilder AddQuotedParameter (object parameter)
		{
			if (parameter != null)
				this.Parameters.AddLast (Quote (FormatObject (parameter)));
			return this;
		}

		public SenseTalkStatementBuilder AddQuotedParameters (object[] parameters)
		{
			if (parameters != null) foreach (object parameter in parameters)
				AddQuotedParameter (parameter);
			return this;
		}

		public SenseTalkStatementBuilder AddPropertyListParameter (string propertyName, object propertyValue)
		{
			if (propertyName != null && propertyValue != null)
				this.PropertyListParameters.Add (propertyName, propertyValue);
			return this;
		}

		public SenseTalkStatementBuilder AddPropertyListParameters (Dictionary<string, object> dictionary)
		{
			if (dictionary != null) foreach(KeyValuePair<string, object> e in dictionary)
				AddPropertyListParameter (e.Key, e.Value);
			return this;
		}

		public SenseTalkStatementBuilder AddQuotedPropertyListParameter (string propertyName, object propertyValue)
		{
			if (propertyName != null && propertyValue != null)
				this.PropertyListParameters.Add (propertyName, Quote (FormatObject (propertyValue)));
			return this;
		}

		public SenseTalkStatementBuilder AddQuotedPropertyListParameters (Dictionary<string, object> dictionary)
		{
			if (dictionary != null) foreach(KeyValuePair<string, object> e in dictionary)
				AddQuotedPropertyListParameter (e.Key, e.Value);
			return this;
		}

		public override string ToString ()
		{
			string s = null;

			switch (this.StatementType)
			{
			case SenseTalkStatementType.Command:
				{
					s = BuildCommandStatement (
						NameOrExpression.ToString (),
						Parameters,
						PropertyListParameters);
					break;
				}
			case SenseTalkStatementType.Function:
				{
					s = BuildFunctionStatement (
						NameOrExpression.ToString (),
						Parameters,
						PropertyListParameters);
					break;
				}
			default:
				{
					s = BuildExpressionStatement (NameOrExpression);
					break;
				}
			}

			return s;
		}

		public static string Quote(string s)
		{
			return "\"" + s.Replace ("\"", "\" & quote & \"") + "\"";
		}

		public static string[] Quote(string[] strings)
		{
			if (strings == null)
				return null;

			var mStrings = new List<string> ();
			foreach (string s in strings)
				mStrings.Add (Quote (s));

			return mStrings.ToArray ();
		}

		public static string FormatObject(object o)
		{
			string s;
			IEnumerable enumerable;
			bool? b;
			Point? p;
			Rectangle? r;
			Size? size;
			KeyValuePair<object, object>? kvp;
			KeyValuePair<string, object>? skvp;

			if ((s = o as string) != null)
			{
				return s;
			} 
			else if ((b = o as bool?) != null)
			{
				s =  b.Value ? "true" : "false";
			}
			else if ((p = o as Point?) != null)
			{
				s = "(" + p.Value.X + ", " + p.Value.Y + ")";
			}
			else if ((size = o as Size?) != null)
			{
				s = "(" + size.Value.Width + ", " + size.Value.Height + ")";
			}
			else if ((r = o as Rectangle?) != null)
			{
				var p1 = r.Value.Location;
				var p2 = new Point(r.Value.Left + r.Value.Width, r.Value.Top + r.Value.Height);
				s = "(" + FormatObject (p1) + ", " + FormatObject (p2) + ")";
			}
			else if ((skvp = o as KeyValuePair<string, object>?) != null)
			{
				s = skvp.Value.Key + ": " + FormatObject (skvp.Value.Value);
			}
			else if ((kvp = o as KeyValuePair<object, object>?) != null)
			{
				s = kvp.Value.Key + ": " + FormatObject (kvp.Value.Value);
			}
			else if ((enumerable = o as IEnumerable) != null)
			{
				bool first = true;
				var sb = new StringBuilder ("(");

				foreach (object element in enumerable) {
					if (first) first = false;
					else
						sb.Append (", ");

					sb.Append (FormatObject (element));
				}

				s = sb.Append (")").ToString ();
			}
			else if (o != null) {
				s = o.ToString ();
			}

			return s;
		}

		public static string BuildExpressionStatement (object expression)
		{
			return (new StringBuilder ("return "))
				.Append (FormatObject(expression))
				.ToString ();
		}

		public static string BuildCommandStatement (
			string commandName,
			LinkedList<object> parameters,
			Dictionary<string, object> propertyListParameters)
		{
			var stringBuilder = new StringBuilder(commandName);

			if (propertyListParameters.Count > 0) {
				stringBuilder.Append (" (");
			} else if (parameters.Count > 0) {
				stringBuilder.Append (" ");
			}

			BuildArgumentList (stringBuilder, parameters, propertyListParameters);

			if (propertyListParameters.Count > 0)
				stringBuilder.Append (")");

			return stringBuilder.ToString();
		}

		public static string BuildFunctionStatement (
			string functionName,
			LinkedList<object> parameters,
			Dictionary<string, object> propertyListParameters)
		{
			var sb = (new StringBuilder(functionName)).Append (" (");
			BuildArgumentList (sb, parameters, propertyListParameters);
			return BuildExpressionStatement (sb.Append (")").ToString ());
		}

		private static void BuildArgumentList (
			StringBuilder stringBuilder,
			LinkedList<object> parameters,
			Dictionary<string, object> propertyListParameters)
		{
			bool firstElementPrinted = false;

			// ordered parameters before property list parameters
			foreach (object parameter in parameters)
			{
				if (firstElementPrinted)
					stringBuilder.Append (", ");
				else
					firstElementPrinted = true;

				stringBuilder.Append (FormatObject (parameter));
			}

			// property list parameters
			foreach(KeyValuePair<string, object> entry in propertyListParameters)
			{
				if (firstElementPrinted)
					stringBuilder.Append (", ");
				else
					firstElementPrinted = true;

				stringBuilder.Append (entry.Key + ": " + FormatObject (entry.Value));
			}
		}

	}
}


/**
 * JSONFormatter.cs
 * 
 * Description:
 *  
 * 
 * Author: 
 *  Pierre-Luc Poirier
 * 
 * Source:
 * 	Mark, http://stackoverflow.com/questions/4580397/json-formatter-in-c
 * 
 * © 2015 Pierre-Luc Poirier. All Rights Reserved.
 */

using System.Text;

public class JsonFormatter
{
	public static string INDENT_VALUE = "    ";
	
	public static string FormatJSON(string json)
	{
		StringBuilder output = new StringBuilder(json.Length * 2);
		char? quote = null;
		int depth = 0;
		
		for (int i = 0; i < json.Length; ++i)
		{
			char ch = json[i];
			
			switch (ch)
			{
			case '{':
			case '[':
				output.Append(ch);
				if (!quote.HasValue)
				{
					output.AppendLine();
					output.Append(Repeat(INDENT_VALUE, ++depth));
				}
				break;
			case '}':
			case ']':
				if (quote.HasValue) 
				{
					output.Append(ch);
				}
				else
				{
					output.AppendLine();
					output.Append(Repeat(INDENT_VALUE, --depth));
					output.Append(ch);
				}
				break;
			case '"':
			case '\'':
				output.Append(ch);
				if (quote.HasValue)
				{
					if (!IsEscaped(output, i))
					{
						quote = null;
					}
				}
				else quote = ch;
				break;
			case ',':
				output.Append(ch);
				if (!quote.HasValue)
				{
					output.AppendLine();
					output.Append(Repeat(INDENT_VALUE, depth));
				}
				break;
			case ':':
				if (quote.HasValue) 
				{
					output.Append(ch);
				}
				else 
				{
					output.Append(": ");
				}
				break;
			default:
				if (quote.HasValue || !char.IsWhiteSpace(ch))
				{
					output.Append(ch);
				}
				break;
			}
		}
		
		return output.ToString();
	}
	
	public static string Repeat(string str, int count)
	{
		return new StringBuilder().Insert(0, str, count).ToString();
	}
	
	public static bool IsEscaped(string str, int index)
	{
		bool escaped = false;
		while (index > 0 && str[--index] == '\\') escaped = !escaped;
		return escaped;
	}
	
	public static bool IsEscaped(StringBuilder str, int index)
	{
		return IsEscaped(str.ToString(), index);
	}
}

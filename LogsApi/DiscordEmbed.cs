using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogsApi;

public class DColor 
{
    public byte R {get; set;}
    public byte G {get; set;}
    public byte B {get; set;}
    public DColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
}
public class Field
{
    public string Name { get; set; }
    public string Value { get; set; }
    public bool InLine { get; set; }
    public Field(string name, string value, bool inLine)
    {
        Name = name;
        Value = value;
        InLine = inLine;
    }
    public Field(Field field)
    {
        Name = field.Name;
        Value = field.Value;
        InLine = field.InLine;
    }
}
public class Author
{
    public string Name { get; set; }
    public string? IconUrl { get; set; }
    public string? Url { get; set; }
    public Author(string name, string? iconUrl = null, string? url = null)
    {
        Name = name;
        IconUrl = iconUrl;
        Url = url;
    }
    public Author(Author author)
    {
        Name = author.Name;
        IconUrl = author.IconUrl;
        Url = author.Url;
    }
}
public class DiscordEmbed
{
    public Author? Author { get; set; } = null;
    public List<Field> Fields { get; set; } = new List<Field>();
    public DColor Color = new (255, 255, 255);
    private Dictionary<string, object> KeyValues { get; set; } = new();

    public string ReplaceKeyValues(string text)
    {
        foreach (var kv in KeyValues)
        {
            text = text.Replace("{" + kv.Key + "}", kv.Value.ToString());
        }
        return text;
    }

    public List<Field> GetFields()
    {
        var fields = new List<Field>();

        foreach (var f in Fields)
        {
            var newField = new Field(f);
            newField.Value = ReplaceKeyValues(newField.Value);
            newField.Name = ReplaceKeyValues(newField.Name);
            fields.Add(newField);
        }

        return fields;
    }
    public Author? GetAuthor()
    {
        if (Author == null)
        {
            return null;
        }
        var newAuthor = new Author(Author);
        newAuthor.Name = ReplaceKeyValues(newAuthor.Name);
        newAuthor.IconUrl = newAuthor.IconUrl != null ? ReplaceKeyValues(newAuthor.IconUrl) : null;
        newAuthor.Url = newAuthor.Url != null ? ReplaceKeyValues(newAuthor.Url) : null;
        return newAuthor;
    }
    public void AddField(string name, string value, bool inLine = false)
    {
        Fields.Add(new Field(name, value, inLine));
    }
    public void WithColor(byte r, byte g, byte b)
    {
        Color = new DColor(r, g, b);
    }
    public void WithAuthor(string name, string? iconUrl = null, string? url = null)
    {
        Author = new Author(name, iconUrl, url);
    }
    public void SetKeyValues(string[] keys, params object[] values)
    {
        KeyValues.Clear();
        for (int i = 0; i < keys.Length; i++) 
        { 
            KeyValues[keys[i]] = values[i];
        }
    }
}
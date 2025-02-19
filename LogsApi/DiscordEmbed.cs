using System.Text.Json.Serialization;

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
    [JsonConstructor]
    public Field(string name = "᲼", string value = "᲼", bool inLine = false)
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
    [JsonConstructor]
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

public class Footer(string? iconUrl = null, string? text = null) {
    public string? IconUrl {get; set;} = iconUrl;
    public string? Text {get; set;} = text;
}

public class DiscordEmbed
{
    // WithTitle()
    public string? Title {get ; set;} = null;
    // WithDescription()
    public string? Description {get ; set;} = null;
    // WithThumbnailUrl()
    public string? ThumbnailUrl {get ; set;} = null;
    // WithTimestamp()
    public DateTimeOffset? Timestamp {get; set;} = null;
    // WithUrl()
    public string? Url {get ; set;} = null;
    // WithImageUrl()
    public string? ImageUrl {get ; set;} = null;


    public Footer? Footer {get; set;} = null;
    public Author? Author { get; set; } = null;
    public List<Field> Fields { get; set; } = new List<Field>();
    public DColor Color {get; set;} = new (255, 255, 255);
    [JsonIgnore]
    private Dictionary<string, object> KeyValues { get; set; } = new();
    [JsonConstructor]
    public DiscordEmbed(
        List<Field> fields, 
        DColor color,
        Author? author = null,
        string? title = null,
        string? thumbnailUrl = null,
        string? url = null,
        string? imageUrl = null,
        Footer? footer = null
        )
    {
        Author = author;
        Fields = fields;
        Color = color;
        Title = title;
        ThumbnailUrl = thumbnailUrl;
        Url = url;
        ImageUrl = imageUrl;
        Footer = footer;
    }
    public DiscordEmbed() {}

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
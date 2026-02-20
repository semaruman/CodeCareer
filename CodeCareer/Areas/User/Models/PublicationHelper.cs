using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CodeCareer.Areas.User.Models
{
    public static class PublicationHelper
    {
        public static HtmlString GeneratePublication(this IHtmlHelper html, PublicationModel publication)
        {
            TagBuilder div = new TagBuilder("div");
            TagBuilder h4 = new TagBuilder("h4");
            TagBuilder a = new TagBuilder("a");
            TagBuilder p = new TagBuilder("p");

            a.Attributes.Add("asp-area", "User");
            a.Attributes.Add("asp-controller", "Home");
            a.Attributes.Add("asp-action", "AlienProfile");
            a.Attributes.Add("asp-route-userEmail", publication.User.Email);
            a.InnerHtml.Append(publication.User.FullName);

            h4.InnerHtml.AppendHtml(a);
            h4.InnerHtml.Append($" - {publication.CreatedDate:dd.MM.yyyy}");

            p.InnerHtml.Append(publication.Content);

            div.InnerHtml.AppendHtml(h4);
            div.InnerHtml.AppendHtml(p);

            using var writer = new StringWriter();
            div.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}

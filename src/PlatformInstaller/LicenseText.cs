using Resourcer;
using Strike.IE;

public static class LicenseText
{
    public static string ReadLicenseHtml()
    {
        using (var markdownify = new Markdownify())
        {
            var innerHtml = markdownify.Transform(ReadLicenseMarkDown());
            return string.Format(@"<!DOCTYPE html>
<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
<meta charset='utf-8' />
<meta http-equiv='X-UA-Compatible' content='IE=edge' /> 
    <style type='text/css'>
        html *
        {{
            font-size: 1em !important;
            color: #000 !important;
            font-family: 'Segoe UI' !important;
        }}
    </style>
</head>
<body>{0}
</body>
</html>", innerHtml);
        }
    }
    public static string ReadLicenseMarkDown()
    {
        return Resource.AsString("LicenseAgreement.md");
    }
}
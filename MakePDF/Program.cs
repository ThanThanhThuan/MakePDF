using PdfHelper;
using System.Diagnostics;
using System.Text;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
var file1 = Path.Combine(baseDirectory, "out1.pdf");
var file2 = Path.Combine(baseDirectory, "out2.pdf");
var file3 = Path.Combine(baseDirectory, "out3.pdf");
string filepath = "";
string r = "1";
try
{

    while (r == "1" || r == "2" || r == "3")
    {
        Console.WriteLine("Press a number:");
        Console.WriteLine("1.Create (overwrite) file: " + file1);
        Console.WriteLine("2.Create (overwrite) file: " + file2);
        Console.WriteLine("3.Create (overwrite) file: " + file3);
        Console.WriteLine("0.Skip to Read File");

        r = Console.ReadKey().KeyChar.ToString();
        if (r != null)
        {
            switch (r)
            {
                case "1":
                    filepath = file1;
                    break;
                case "2":
                    filepath = file2;
                    break;
                case "3":
                    filepath = file3;
                    break;
                default:
                    filepath = "";
                    break;
            }
        }
        if (filepath != "")
        {
            //Create a Catalog Dictionary
            CatalogDict catalogDict = new CatalogDict();

            //Create a Page Tree Dictionary
            PageTreeDict pageTreeDict = new PageTreeDict();

            //Create a Font Dictionary
            FontDict TimesRoman = new FontDict();
            FontDict TimesItalic = new FontDict();

            //Create the info Dictionary
            InfoDict infoDict = new InfoDict();

            //Create the font called Times Roman
            TimesRoman.CreateFontDict("T1", "Times-Roman");

            //Create the font called Times Italic
            TimesItalic.CreateFontDict("T2", "Times-Italic");

            //Set the info Dictionary. 
            infoDict.SetInfo("title", "author", "company");

            //Create a utility object
            Utility pdfUtility = new Utility();

            //Open a file specifying the file name as the output pdffile 
            FileStream file = new FileStream(filepath, FileMode.Create);


            int size = 0;
            file.Write(pdfUtility.GetHeader("1.5", out size), 0, size);
            file.Close();

            //Create a Page Dictionary , this represents a visible page
            PageDict page = new PageDict();
            ContentDict content = new ContentDict();

            //The page size object will hold all the page size information
            PageSize pSize = new PageSize(612, 792);
            pSize.SetMargins(10, 10, 10, 10);
            page.CreatePage(pageTreeDict.objectNum, pSize);
            pageTreeDict.AddPage(page.objectNum);
            page.AddResource(TimesRoman, content.objectNum);


            //Create a Text And Table Object that present the elements in the page
            TextAndTables textAndtable = new TextAndTables(pSize);
            string txt = "";
            Console.WriteLine();
            Console.WriteLine("Enter a text for " + Path.GetFileName(filepath) + ":");
            txt = Console.ReadLine().Trim();
            textAndtable.AddText(20, 10, txt, 10, "T1", Align.CenterAlign);

            content.SetStream(textAndtable.EndText());

            size = 0;
            file = new FileStream(filepath, FileMode.Append);
            file.Write(page.GetPageDict(file.Length, out size), 0, size);
            file.Write(content.GetContentDict(file.Length, out size), 0, size);
            file.Close();
            //Write everything file size=0;
            file = new FileStream(filepath, FileMode.Append);
            file.Write(catalogDict.GetCatalogDict(pageTreeDict.objectNum,
                                              file.Length, out size), 0, size);
            file.Write(pageTreeDict.GetPageTree(file.Length, out size), 0, size);
            file.Write(TimesRoman.GetFontDict(file.Length, out size), 0, size);
            file.Write(TimesItalic.GetFontDict(file.Length, out size), 0, size);

            file.Write(infoDict.GetInfoDict(file.Length, out size), 0, size);
            file.Write(pdfUtility.CreateXrefTable(file.Length, out size), 0, size);
            file.Write(pdfUtility.GetTrailer(catalogDict.objectNum,
                                       infoDict.objectNum, out size), 0, size);

            file.Close();
            Console.WriteLine("File created: " + filepath);
        }
        Console.WriteLine();
        Console.WriteLine("Press a number:");
        Console.WriteLine("1.Read file: " + file1);
        Console.WriteLine("2.Read file: " + file2);
        Console.WriteLine("3.Read file: " + file3);
        Console.WriteLine("Other key to exit");
        r = "";
        filepath = "";
        r = Console.ReadKey().KeyChar.ToString();
        if (r != null)
        {
            Console.WriteLine();
            switch (r)
            {
                case "1":
                    filepath = file1;
                    break;
                case "2":
                    filepath = file2;
                    break;
                case "3":
                    filepath = file3;
                    break;
                default:
                    filepath = "";
                    break;
            }
        }
        if (filepath != "")
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("File does not exist: " + filepath + ". You can create or copy file to there.");
                filepath = "";
            }
            else
            {
                using var sr = new StreamReader(filepath);
                string? line;
                bool BT = false; //Pdf BT operator to begin text

                string groupfound = "";
                string textfound = "";
                string groupfound0 = "";
                //  string textfound0 = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("BT"))
                    {
                        BT = true;

                    }

                    if (BT)
                    {
                        if (line.StartsWith("ET"))
                        {
                            BT = false;
                            groupfound0 += "ET\r\n";
                            groupfound += groupfound0;
                            groupfound0 = "";
                        }
                        else
                        {
                            groupfound0 += line + "\r\n";
                            if (line.Contains("("))
                            {
                                int start = line.IndexOf("(") + 1;
                                int length = line.LastIndexOf(")") - start;
                                line = line.Substring(start, length);

                                textfound += line + "\r\n";
                            }
                        }

                    }


                }
                if (textfound != "")
                {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>");
                    Console.WriteLine("Text found in " + Path.GetFileName(filepath) + ":");
                    Console.WriteLine(textfound);
                    Console.WriteLine("Location:");
                    Console.WriteLine(groupfound);
                }
                // r = Console.ReadKey().KeyChar.ToString();

            }
            Console.WriteLine("==================================================");
        }
    }

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.ReadKey();

}
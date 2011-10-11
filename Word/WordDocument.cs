using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using ProcessCapture.Screenshot;
using System.Diagnostics;

namespace ProcessCapture
{
    public class WordDocument
    {
        static object oMissing = System.Reflection.Missing.Value;
        static object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */
        static object oPageBreak = Word.WdBreakType.wdPageBreak;

        public static void CreateDocument(string title, string subtitle, string description, string author,
            string processImagePath, List<ScreenImage> images, string outputFileName, bool includeModules, bool format2003)
        {
            object template = GetTemplateDirectory(format2003);

            if (File.Exists(template.ToString()))
            {
                //Start Word and create a new document.
                Word._Application oWord = new Word.Application();
                Word._Document oDoc;
                object visible = true;

                oDoc = oWord.Documents.Add(ref template, ref oMissing, ref oMissing, ref visible);

                UpdateBookmark(subtitle, "processtitle", oDoc, 18, 1);
                UpdateBookmark(author, "author", oDoc, 16, 0);
                UpdateBookmark(subtitle, "processtitle2", oDoc, 18, 1);
                UpdateBookmark(description, "description", oDoc, 11, 0);
                UpdateBookmarkWithImage(processImagePath, "processimage", oDoc);

                foreach (ScreenImage si in images)
                {
                    UpdateBookmark(si.Title, oEndOfDoc, oDoc, 16, 1);
                    UpdateBookmark("URL: " + si.ApplicationURL, oEndOfDoc, oDoc, 11, 0);
                    UpdateBookmark("Path: " + si.Path, oEndOfDoc, oDoc, 11, 0);
                    UpdateBookmark("Screenshot taken at " + si.Timestamp, oEndOfDoc, oDoc, 11, 0);
                    UpdateBookmark(si.Notes, oEndOfDoc, oDoc, 11, 0);
                    UpdateBookmarkWithImage(si.Filename, oEndOfDoc, oDoc);
                    InsertPageBreak(oDoc);
                }

                object output = outputFileName;
                oDoc.SaveAs(ref output, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //oWord.Visible = true;
                oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
            }
        }

        private static void InsertPageBreak(Word._Document oDoc)
        {
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            wrdRng.InsertBreak(ref oPageBreak);
        }

        private static void UpdateBookmark(string text, object bookmark, Word._Document oDoc, int size, int bold)
        {
            if (oDoc.Bookmarks.Exists(bookmark.ToString()))
            {
                object wrdRng = oDoc.Bookmarks.get_Item(ref bookmark).Range;

                Word.Paragraph oPara2;
                oPara2 = oDoc.Content.Paragraphs.Add(ref wrdRng);
                oPara2.Range.Text = text;
                oPara2.Format.SpaceAfter = 6;
                oPara2.Range.Font.Size = size;
                oPara2.Range.Font.Bold = bold;
                oPara2.Range.InsertParagraphAfter();
            }
        }

        private static void UpdateBookmarkWithImage(string path, object bookmark, Word._Document oDoc)
        {
            if (oDoc.Bookmarks.Exists(bookmark.ToString()) && File.Exists(path))
            {
                object wrdRng = oDoc.Bookmarks.get_Item(ref bookmark).Range;
                Object oLinkToFile = false;  //default
                Object oSaveWithDocument = true;//default

                oDoc.InlineShapes.AddPicture(path, ref oLinkToFile, ref oSaveWithDocument, ref wrdRng); 
            }
        }

        private static string GetTemplateDirectory(bool format2003)
        {
            string dirName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            if(format2003)
                return dirName + "\\Template.dot";
            else
                return dirName + "\\Template.dotx";
        }
    }
}

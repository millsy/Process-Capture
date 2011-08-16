using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessCapture.Screenshot;
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using System.IO;
using System.Reflection;
using TallComponents.PDF.Layout.Fonts;
using System.Windows;
using TallComponents.PDF.Layout.Navigation;

namespace ProcessCapture.PDF
{
    public class PDFDocument
    {

        public static void CreateDocument(string title, string subtitle, string description, string author, 
            string processImagePath, List<ScreenImage> images, string outputFileName, bool includeModules)
        {
            Document doc = new Document();
            
            SetupFrontPage(doc, title, subtitle);
            SetupIntroductionPage(doc, subtitle, author, description, processImagePath);

            foreach (ScreenImage si in images)
            {
                Section s = new Section();
                s.StartOnNewPage = true;
                doc.Sections.Add(s);

                TextParagraph tp = new TextParagraph();
                tp.SpacingBefore = 2;
                tp.SpacingAfter = 20;
                s.Paragraphs.Add(tp);

                Fragment f = new Fragment();
                f.Text = si.Title + Environment.NewLine; 
                f.Bold = true;
                f.Font = Font.HelveticaBold;
                f.FontSize = 14;
                f.PreserveWhiteSpace = true;
                tp.Fragments.Add(f);

                f = new Fragment();
                f.Text = "Program path: " + si.Path + Environment.NewLine ;
                f.Bold = true;
                f.Font = Font.Helvetica;
                f.FontSize = 13;
                f.PreserveWhiteSpace = true;
                tp.Fragments.Add(f);

                if (si.ApplicationURL != null)
                {
                    f = new Fragment();
                    f.Text = "URL: " + si.ApplicationURL + Environment.NewLine;
                    f.Bold = true;
                    f.Font = Font.Helvetica;
                    f.FontSize = 13;
                    f.PreserveWhiteSpace = true;
                    tp.Fragments.Add(f);
                }

                f = new Fragment();
                f.Text = si.Notes + Environment.NewLine;
                f.Bold = true;
                f.Font = Font.Helvetica;
                f.FontSize = 13;
                f.PreserveWhiteSpace = true;
                tp.Fragments.Add(f);

                Section imageSec = new Section();
                doc.Sections.Add(imageSec);

                Image i = new Image(si.Bitmap, false);
                i.SpacingBefore = 2;
                i.SpacingAfter = 2;
                i.FitPolicy = FitPolicy.Shrink;

                imageSec.Paragraphs.Add(i);

                if (includeModules)
                {
                    Section modules = new Section();
                    modules.StartOnNewPage = true;
                    doc.Sections.Add(modules);

                    TextParagraph modulesTP = new TextParagraph();
                    modulesTP.SpacingBefore = 2;
                    modulesTP.SpacingAfter = 20;
                    modules.Paragraphs.Add(modulesTP);

                    f = new Fragment();
                    f.Text = "Modules" + Environment.NewLine;
                    f.Bold = true;
                    f.Font = Font.HelveticaBold;
                    f.FontSize = 14;
                    f.PreserveWhiteSpace = true;
                    modulesTP.Fragments.Add(f);

                    if (si.Modules != null)
                    {
                        foreach (string module in si.Modules)
                        {
                            Fragment moduleFrag = new Fragment();
                            moduleFrag.Text = module + Environment.NewLine;
                            moduleFrag.Bold = false;
                            moduleFrag.Font = Font.Courier;
                            moduleFrag.FontSize = 13;
                            moduleFrag.PreserveWhiteSpace = true;
                            moduleFrag.KeepWithNext = false;
                            modulesTP.Fragments.Add(moduleFrag);
                        }
                    }
                }
            }

            using (FileStream fs = new FileStream(outputFileName, FileMode.Create))
            {
                doc.Write(fs);
            }
        }

        private static void SetupIntroductionPage(Document doc, string subtitle, string author, string description, string processImagePath)
        {
			if(author == null)
				author = "";
			if(description == null)
				description = "";
			if(subtitle == null)
				subtitle = "";
			if(processImagePath == null)
				processImagePath = "";
			
            Section introPage = new Section();
            introPage.StartOnNewPage = true;

            doc.Sections.Add(introPage);

            TextParagraph tp = new TextParagraph();
            tp.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
            Fragment f = new Fragment();
            f.Text = subtitle;
            f.Font = Font.HelveticaBold;
            f.FontSize = 22;
            tp.Fragments.Add(f);

            tp.SpacingBefore = 2;
            tp.SpacingAfter = 2;

            introPage.Paragraphs.Add(tp);

            tp = new TextParagraph();
            tp.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
            f = new Fragment();
            f.Text = "Author: " + author;
            f.Font = Font.HelveticaBold;
            f.FontSize = 14;
            f.PreserveWhiteSpace = true;
            f.Bold = true;
            f.Underline = true;

            tp.Fragments.Add(f);
            tp.SpacingBefore = 2;
            tp.SpacingAfter = 20;

            introPage.Paragraphs.Add(tp);

            tp = new TextParagraph();
            tp.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Left;
            f = new Fragment();
            f.Text = description;
            f.Font = Font.HelveticaBold;
            f.FontSize = 12;
            f.PreserveWhiteSpace = true;
            tp.Fragments.Add(f);

            introPage.Paragraphs.Add(tp);

			if(processImagePath != string.Empty)
			{
				Section imagePage = new Section();
				imagePage.StartOnNewPage = true;
	
				doc.Sections.Add(imagePage);
	
				Image pi = new Image(processImagePath);
				pi.SpacingAfter = 20;
				pi.SpacingBefore = 20;
				pi.KeepAspectRatio = true;
				pi.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
				pi.FitPolicy = FitPolicy.Shrink;
				pi.Caption = "Process diagram";
	
				imagePage.Paragraphs.Add(pi);
			}

        }

        private static void SetupFrontPage(Document doc, string title, string subtitle)
        {
			if(title == null)
				title = "";
			if(subtitle == null)
				subtitle = "";
			
            ViewerPreferences vp = new ViewerPreferences();
            vp.ZoomFactor = 0.75;
            doc.ViewerPreferences = vp;

            Section mainPage = new Section();
            mainPage.StartOnNewPage = true;

            doc.Sections.Add(mainPage);

			System.Drawing.Bitmap logo = GetOpenSpanLogo();
			if(logo != null)
			{
				Image i = new Image(logo, true);
				i.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
				i.FitPolicy = FitPolicy.Shrink;
				i.Compression = Compression.Auto;
				i.KeepAspectRatio = true;
				i.Actions.Add(new TallComponents.PDF.Layout.Actions.UriAction("www.openspan.com"));
	
				double w = (mainPage.PageSize.Width/2) - (i.Width / 2);
				double h = (mainPage.PageSize.Height/2) - (i.Height / 2);
	
				Area a = new Area(w, mainPage.PageSize.Height, i.Width, mainPage.PageSize.Height);
				a.VerticalAlignment = TallComponents.PDF.Layout.VerticalAlignment.Middle;
				
				mainPage.ForegroundAreas.Add(a);
	
				a.Paragraphs.Add(i);

				TextParagraph tp = new TextParagraph();
				tp.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
				Fragment f = new Fragment();
				f.Text = title;
				f.Font = Font.HelveticaBold;
				f.FontSize = 22;
				f.Bold = true;
				tp.Fragments.Add(f);
				a.Paragraphs.Add(tp);
	
				tp = new TextParagraph();
				tp.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
				f = new Fragment();
				f.Text = subtitle;
				f.Bold = true;
				f.Font = Font.HelveticaBold;
				f.FontSize = 18;
				tp.Fragments.Add(f);
	
				a.Paragraphs.Add(tp);
			}
			
        }

        private static System.Drawing.Bitmap GetOpenSpanLogo()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            System.Drawing.Bitmap megatron = null;
            using (Stream resStream = Application.GetResourceStream(new Uri("Images/openspanLogo.png", UriKind.Relative)).Stream)//asm.GetManifestResourceStream("ProcessCapture.Images.openspanLogo.png"))
            {
				if(resStream != null)
                	megatron = new System.Drawing.Bitmap(resStream);
            }
            return megatron;
        }
    }
}

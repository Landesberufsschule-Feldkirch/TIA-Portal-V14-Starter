﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace TIA_Portal_V14_SP1_Starter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            try
            {
                uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
            catch (Exception exp)
            {
                Console.WriteLine("{0} Exception 1 caught.", exp);
            }
        }
    }

    public partial class MainWindow : Window
    {
        bool AnzeigeAktualisieren = false;
        string ProjektName = "";
        string ProjektPfad = "h:\\TiaPortal_V14";
        List<RadioButton> RadioButtonList = new List<RadioButton>();
        List<Button> ButtonListe = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();
            ProjekteLesen();
        }

        public void ProjekteLesen()
        {

            /*
            * Aufbau der Projektnamen (Ordner)
            * V14SP1_PLC_WEB_FUP_Linearachse
            * 
            * _PLC_ oder  _BUG_    
            * + _NC_    .. Servo
            * + _HMI    .. HMI (Touch)
            * + _VISU_  .. Visualisierung
            * + _FIO_   .. Factory I/O
            * + _WEB_   .. Internet
            * + _DT_    .. digital Twin
            * 
            * _AWL_ oder _AS_ oder _FUP_ oder _KOP_ oder _SCL_ oder _ST_
            * 
            * */

            // Zuerst die Listen löschen
            StackPanel_BUG.Children.Clear();
            StackPanel_PLC.Children.Clear();
            StackPanel_PLC_FIO.Children.Clear();
            StackPanel_PLC_HMI.Children.Clear();
            StackPanel_DigitalTwin.Children.Clear();
            //
            ButtonListe.Add(ProjektStarten_BUG);
            ButtonListe.Add(ProjektStarten_PLC);
            ButtonListe.Add(ProjektStarten_PLC_FIO);
            ButtonListe.Add(ProjektStarten_PLC_HMI);
            ButtonListe.Add(ProjektStarten_DigitalTwin);

            // Name Komplett, kurz, Sprache, Anfang
            List<Tuple<string, string, string>> TupleList_PLC = new List<Tuple<string, string, string>>();
            List<Tuple<string, string, string>> TupleList_PLC_FIO = new List<Tuple<string, string, string>>();
            List<Tuple<string, string, string>> TupleList_PLC_HMI = new List<Tuple<string, string, string>>();
            List<Tuple<string, string, string>> TupleList_DigitalTwin = new List<Tuple<string, string, string>>();
            List<Tuple<string, string, string>> TupleList_BUG = new List<Tuple<string, string, string>>();

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            foreach (System.IO.DirectoryInfo d in ParentDirectory.GetDirectories())
            {
                string OrdnerName = d.Name;
                string Sprache = "";
                int StartBezeichnung = 0;
                bool Anzeigen = false;

                if (OrdnerName.Contains("FUP"))
                {
                    if (Checkbox_FUP.IsChecked.Value)
                    {
                        Anzeigen = true;
                    }
                    Sprache = "FUP";
                    StartBezeichnung = 4 + OrdnerName.IndexOf("FUP");
                }
                if (OrdnerName.Contains("KOP"))
                {
                    if (Checkbox_KOP.IsChecked.Value)
                    {
                        Anzeigen = true;
                    }
                    Sprache = "KOP";
                    StartBezeichnung = 4 + OrdnerName.IndexOf("KOP");
                }
                if (OrdnerName.Contains("SCL"))
                {
                    if (Checkbox_SCL.IsChecked.Value)
                    {
                        Anzeigen = true;
                    }
                    Sprache = "SCL";
                    StartBezeichnung = 4 + OrdnerName.IndexOf("SCL");
                }

                if (Anzeigen)
                {
                    if (d.Name.Contains("PLC"))
                    {
                        if (d.Name.Contains("DT"))
                        {
                            Tuple<string, string, string> TplEintrag = new Tuple<string, string, string>(OrdnerName.Substring(StartBezeichnung), Sprache, OrdnerName);
                            TupleList_DigitalTwin.Add(TplEintrag);
                        }
                        else
                        {
                            if (d.Name.Contains("HMI"))
                            {
                                Tuple<string, string, string> TplEintrag = new Tuple<string, string, string>(OrdnerName.Substring(StartBezeichnung), Sprache, OrdnerName);
                                TupleList_PLC_HMI.Add(TplEintrag);
                            }
                            else
                            {
                                if (d.Name.Contains("FIO"))
                                {
                                    Tuple<string, string, string> TplEintrag = new Tuple<string, string, string>(OrdnerName.Substring(StartBezeichnung), Sprache, OrdnerName);
                                    TupleList_PLC_FIO.Add(TplEintrag);
                                }
                                else
                                {
                                    // nur PLC und sonst nichts
                                    Tuple<string, string, string> TplEintrag = new Tuple<string, string, string>(OrdnerName.Substring(StartBezeichnung), Sprache, OrdnerName);
                                    TupleList_PLC.Add(TplEintrag);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Es gibt momentan noch keine Gruppe bei den Bugs
                    Tuple<string, string, string> TplEintrag = new Tuple<string, string, string>(OrdnerName.Substring(StartBezeichnung), Sprache, OrdnerName);
                    TupleList_BUG.Add(TplEintrag);
                }

            } // Ende foreach

            TupleList_PLC.Sort();
            TupleList_PLC_FIO.Sort();
            TupleList_PLC_HMI.Sort();
            TupleList_DigitalTwin.Sort();
            TupleList_BUG.Sort();

            TabMitInhaltFuellen(TupleList_PLC, StackPanel_PLC);
            TabMitInhaltFuellen(TupleList_PLC_FIO, StackPanel_PLC_FIO);
            TabMitInhaltFuellen(TupleList_PLC_HMI, StackPanel_PLC_HMI);
            TabMitInhaltFuellen(TupleList_DigitalTwin, StackPanel_DigitalTwin);
            TabMitInhaltFuellen(TupleList_BUG, StackPanel_BUG);

            AnzeigeAktualisieren = true;
        }

        private void TabMitInhaltFuellen(List<Tuple<string, string, string>> Projekte, System.Windows.Controls.StackPanel StackPanel)
        {
            foreach (Tuple<string, string, string> Projekt in Projekte)
            {
                RadioButton rdo = new RadioButton();
                rdo.GroupName = "TIA_PORTAL_V14_SP1";
                rdo.VerticalAlignment = VerticalAlignment.Top;
                rdo.Checked += new RoutedEventHandler(radioButton_Checked);
                rdo.FontSize = 14;

                // nur PLC und sonst nichts
                rdo.Content = Projekt.Item1 + " (" + Projekt.Item2 + ")";
                rdo.Name = Projekt.Item3;
                StackPanel.Children.Add(rdo);
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            DarstellungAendernListe(ButtonListe, true, Colors.Green, "Projekt starten");
            ProjektName = rb.Name;

            string LeereHtmlSeite = "<!doctype html>   </html >";
            string HtmlSeite = "";

            string DateiName = ParentDirectory.FullName + "\\" + rb.Name + "\\index.html";
            if (File.Exists(DateiName))
            {
                HtmlSeite = System.IO.File.ReadAllText(DateiName);
            }
            else
            {
                HtmlSeite = "<!doctype html>   </html >";
            }

            Web_PLC.NavigateToString(LeereHtmlSeite);
            Web_PLC_FIO.NavigateToString(LeereHtmlSeite);
            Web_PLC_HMI.NavigateToString(LeereHtmlSeite);
            Web_DigitalTwin.NavigateToString(LeereHtmlSeite);
            Web_BUG.NavigateToString(LeereHtmlSeite);

            if (rb.Name.Contains("PLC"))
            {
                if (rb.Name.Contains("DT"))
                    Web_DigitalTwin.NavigateToString(HtmlSeite);
                else
                {
                    if (rb.Name.Contains("HMI"))
                        Web_PLC_HMI.NavigateToString(HtmlSeite);
                    else
                    {
                        if (rb.Name.Contains("FIO"))
                            Web_PLC_FIO.NavigateToString(HtmlSeite);
                        else
                        {
                            Web_PLC.NavigateToString(HtmlSeite);
                        }
                    }
                }
            }
            else
            {
                if (rb.Name.Contains("BUG")) Web_BUG.NavigateToString(HtmlSeite);
                //bei Bug gibt es keine Unterkategorien
            }

        }

        private void ProjektStarten(object sender, RoutedEventArgs e)
        {

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");
            string sourceDirectory = ParentDirectory.FullName + "\\" + ProjektName;

            try
            {
                DarstellungAendernListe(ButtonListe, true, Colors.Yellow, "Ordner " + ProjektPfad + " löschen");
                if (System.IO.Directory.Exists(ProjektPfad)) System.IO.Directory.Delete(ProjektPfad, true);
            }
            catch (Exception exp)
            {
                Console.WriteLine("{0} Exception 2 caught.", exp);
            }

            try
            {
                DarstellungAendernListe(ButtonListe, true, Colors.Yellow, "Ordner " + ProjektPfad + " erstellen");
                System.IO.Directory.CreateDirectory(ProjektPfad);
            }
            catch (Exception exp)
            {
                Console.WriteLine("{0} Exception 3 caught.", exp);
            }

            try
            {
                DarstellungAendernListe(ButtonListe, true, Colors.Yellow, "Alle Dateien kopieren");
                Copy(sourceDirectory, ProjektPfad);
            }
            catch (Exception exp)
            {
                Console.WriteLine("{0} Exception 4 caught.", exp);
            }

            try
            {
                DarstellungAendernListe(ButtonListe, true, Colors.LawnGreen, "Projekt mit TIA Portal V14 öffnen");
                Process proc = new Process();
                proc.StartInfo.FileName = ProjektPfad + "\\start.cmd";
                proc.StartInfo.WorkingDirectory = ProjektPfad;
                proc.Start();
            }
            catch (Exception exp)
            {
                Console.WriteLine("{0} Exception 5 caught.", exp);
            }

        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void TabControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DarstellungAendernListe(ButtonListe, false, Colors.Gray, "Projekt auswählen");
            AlleRadioButtonsDeaktivieren();

            string LeereHtmlSeite = "<!doctype html>   </html >";
            Web_PLC.NavigateToString(LeereHtmlSeite);
            Web_PLC_FIO.NavigateToString(LeereHtmlSeite);
            Web_PLC_HMI.NavigateToString(LeereHtmlSeite);
            Web_DigitalTwin.NavigateToString(LeereHtmlSeite);
            Web_BUG.NavigateToString(LeereHtmlSeite);
        }


        private void DarstellungAendernListe(List<Button> KnopfListe, bool Enable, Color Farbe, string Text)
        {
            foreach (Button Knopf in KnopfListe)
            {
                Knopf.IsEnabled = Enable;
                Knopf.Background = new SolidColorBrush(Farbe);
                Knopf.Content = Text;
                Knopf.Refresh();
            }
        }


        private void AlleRadioButtonsDeaktivieren()
        {
            foreach (RadioButton R_Button in RadioButtonList)
            {
                if (R_Button.IsChecked == true) R_Button.IsChecked = false;
            }
        }

        private void Klick_CheckBox_KOP(object sender, RoutedEventArgs e)
        {
            if (AnzeigeAktualisieren) ProjekteLesen();
        }

        private void Klick_CheckBox_FUP(object sender, RoutedEventArgs e)
        {
            if (AnzeigeAktualisieren) ProjekteLesen();
        }

        private void Klick_CheckBox_SCL(object sender, RoutedEventArgs e)
        {
            if (AnzeigeAktualisieren) ProjekteLesen();
        }


    }
}

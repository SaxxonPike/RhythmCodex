using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RhythmCodex.Cli;
using RhythmCodex.Gui.FluentForms;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui
{
    [Service]
    public class FormFactory : IFormFactory
    {
        private readonly IApp _app;
        private readonly IConsoleEventSource _consoleEventSource;
        private readonly IFileDialog _fileDialog;
        private readonly IGuiTasks _guiTasks;

        private static class Ids
        {
            private static string GetNewId() => Guid.NewGuid().ToString();
            
            public static readonly string LogTabId = GetNewId();
            public static readonly string MainFormOutputFolderSelect = GetNewId();
            public static readonly string DdrExtract573GamePath = GetNewId();
            public static readonly string DdrExtract573CardPath = GetNewId();
            public static readonly string DdrExtract573Start = GetNewId();
            public static readonly string MainForm = GetNewId();
            public static readonly string MainFormMenu = GetNewId();
            public static readonly string MainFormTabControl = GetNewId();
            public static readonly string Log = GetNewId();
            public static readonly string DdrDecrypt573AudioPath = GetNewId();
            public static readonly string DdrDecrypt573AudioStart = GetNewId();
        }

        public FormFactory(IApp app, IConsoleEventSource consoleEventSource, IFileDialog fileDialog, IGuiTasks guiTasks)
        {
            _app = app;
            _consoleEventSource = consoleEventSource;
            _fileDialog = fileDialog;
            _guiTasks = guiTasks;
        }

        public Form CreateMainForm()
        {
            var form = new FluentForm
            {
                Id = Ids.MainForm,
                Text = "RhythmCodex GUI",
                Controls = new List<FluentControl>
                {
                    CreateMainFormTabs(),
                    CreateMainFormMenu(),
                },
                AfterBuild = x => { x.Control.MainMenuStrip = x.GetControl<MenuStrip>(Ids.MainFormMenu); }
            };

            var builtForm = (Form) form.Build();
            builtForm.Size = new Size(640, 480);
            return builtForm;
        }

        private FluentControl CreateMainFormLog()
        {
            var textbox = new FluentTextBox
            {
                Dock = DockStyle.Fill,
                Id = Ids.Log,
                MultiLine = true,
                WordWrap = false,
                ScrollBars = ScrollBars.Both,
                Font = new Font(FontFamily.GenericMonospace, 10),
                AfterBuild = x => { _consoleEventSource.Logged += (s, e) => x.Control.Text += e.Text; }
            };

            return textbox;
        }

        private FluentControl CreateMainFormMenu()
        {
            return new FluentMenu
            {
                Id = Ids.MainFormMenu,
                Items = new List<FluentMenuItem>
                {
                    new FluentMenuItem
                    {
                        Text = "&File",
                        Items = new List<FluentMenuItem>
                        {
                            new FluentMenuItem
                            {
                                Text = "Test Execute",
                                OnClick = () => _app.Run()
                            },
                            new FluentMenuItem
                            {
                                Text = "E&xit",
                                OnClick = Application.Exit
                            }
                        }
                    }
                }
            };
        }

        private FluentControl CreateMainFormTabs()
        {
            return new FluentPanel
            {
                Padding = new Padding(8, 2, 8, 8),
                Controls = new List<FluentControl>
                {
                    CreateStandardTable(CreateFolderSelect(Ids.MainFormOutputFolderSelect, "Output Path")
                        .Concat(new[] {CreateMainFormTabControl()}), 2, false)
                }
            };
        }

        private FluentControl CreateMainFormTabControl()
        {
            return new FluentTabControl
            {
                Id = Ids.MainFormTabControl,
                ColumnSpan = 3,
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Id = Ids.LogTabId,
                        Text = "Log",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormLog()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "BMS",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormBmsPage(),
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "beatmania",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormBeatmaniaPage()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "DanceDanceRevolution",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormDanceDanceRevolutionPage()
                        }
                    }
                }
            };
        }

        private FluentControl CreateMainFormBmsPage()
        {
            return new FluentPanel();
        }

        private FluentControl CreateMainFormDanceDanceRevolutionPage()
        {
            return new FluentTabControl
            {
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Text = "Playstation"
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation 2"
                    },
                    new FluentTabPage
                    {
                        Text = "Xbox"
                    },
                    new FluentTabPage
                    {
                        Text = "Xbox 360"
                    },
                    new FluentTabPage
                    {
                        Text = "System 573",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormDanceDanceRevolution573Page()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "Python"
                    },
                    new FluentTabPage
                    {
                        Text = "PC"
                    }
                }
            };
        }

        private FluentControl CreateMainFormDanceDanceRevolution573Page()
        {
            return CreateStandardTable(CreateFileSelect(Ids.DdrExtract573GamePath, "GAME file", false)
                    .Concat(CreateFileSelect(Ids.DdrExtract573CardPath, "CARD file (optional)", false))
                    .Concat(CreateBigButton(Ids.DdrExtract573Start, "Extract 573 flash card files", DdrExtract573Flash))
                    .Concat(CreateFileSelect(Ids.DdrDecrypt573AudioPath, "Encrypted audio files", true))
                    .Concat(CreateBigButton(Ids.DdrDecrypt573AudioStart, "Decrypt 573 audio files", DdrDecrypt573Audio)),
                5);
        }

        private FluentTable CreateStandardTable(IEnumerable<FluentControl> controls, int rows, bool padBottom = true)
        {
            return new FluentTable
            {
                Columns = new List<ColumnStyle>
                {
                    new ColumnStyle(SizeType.Absolute, 175),
                    new ColumnStyle(SizeType.Percent, 100),
                    new ColumnStyle(SizeType.Absolute, 50)
                },
                Rows = Enumerable.Repeat(0, rows)
                    .Select(_ => new RowStyle(SizeType.AutoSize))
                    .Concat(padBottom ? new[] {new RowStyle(SizeType.Percent)} : Array.Empty<RowStyle>())
                    .ToList(),
                Controls = controls.ToList()
            };
        }

        private IEnumerable<FluentControl> CreateBigButton(string id, string text, Action<FluentContext> press)
        {
            return new FluentControl[]
            {
                new FluentEmpty(),
                new FluentButton
                {
                    Id = id,
                    Text = text,
                    Dock = DockStyle.Fill,
                    ColumnSpan = 2,
                    AfterBuild = x => GetContextDelegate(x, press)
                }
            };
        }

        private static void GetContextDelegate<TControl>(
            FluentContext<FluentControl<TControl>, TControl> x, 
            Action<FluentContext> action) 
            where TControl : Control
        {
            x.Blueprint.OnClick = () => action(x);
        }

        private IEnumerable<FluentControl> CreateFileSelect(string id, string text, bool multi)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight
                },
                new FluentTextBox
                {
                    Id = id,
                    Dock = DockStyle.Fill
                },
                new FluentButton
                {
                    Id = id + "Button",
                    Dock = DockStyle.Fill,
                    Text = "...",
                    AfterBuild = x => x.Blueprint.OnClick = () =>
                    {
                        var fileName = _fileDialog.OpenFile(x.Control.Text, null, multi);
                        if (fileName != null)
                            x.GetControl<TextBox>(id).Text = fileName;
                    }
                }
            };
        }

        private IEnumerable<FluentControl> CreateFolderSelect(string id, string text)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight
                },
                new FluentTextBox
                {
                    Id = id,
                    Dock = DockStyle.Fill
                },
                new FluentButton
                {
                    Id = id + "Button",
                    Dock = DockStyle.Fill,
                    Text = "...",
                    AfterBuild = x => x.Blueprint.OnClick = () =>
                    {
                        var folderPath = _fileDialog.OpenFolder(x.Control.Text);
                        if (folderPath != null)
                            x.GetControl<TextBox>(id).Text = folderPath;
                    }
                }
            };
        }

        private FluentControl CreateMainFormBeatmaniaPage()
        {
            return new FluentTabControl
            {
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Text = "Playstation"
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation 2"
                    },
                    new FluentTabPage
                    {
                        Text = "Djmain"
                    },
                    new FluentTabPage
                    {
                        Text = "Twinkle"
                    },
                    new FluentTabPage
                    {
                        Text = "Firebeat"
                    },
                    new FluentTabPage
                    {
                        Text = "PC"
                    }
                }
            };
        }

        private static void ShowLogTab(FluentContext context)
        {
            context.GetControl<TabPage>(Ids.LogTabId).Select();
        }
        
        private void DdrExtract573Flash(FluentContext context)
        {
            ShowLogTab(context);
            _guiTasks.DdrExtract573Flash(
                context.GetControl<TextBox>(Ids.DdrExtract573GamePath).Text,
                context.GetControl<TextBox>(Ids.DdrExtract573CardPath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text);
        }

        private void DdrDecrypt573Audio(FluentContext context)
        {
            ShowLogTab(context);
            _guiTasks.DdrDecrypt573Audio(
                context.GetControl<TextBox>(Ids.DdrDecrypt573AudioPath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text);
        }
    }
}
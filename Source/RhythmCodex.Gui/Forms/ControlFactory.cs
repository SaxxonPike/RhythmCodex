using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RhythmCodex.Gui.FluentForms;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms
{
    [Service]
    public class ControlFactory : IControlFactory
    {
        private readonly IFileDialog _fileDialog;
        private readonly IFontFactory _fontFactory;

        public ControlFactory(IFileDialog fileDialog, IFontFactory fontFactory)
        {
            _fileDialog = fileDialog;
            _fontFactory = fontFactory;
        }

        public IEnumerable<FluentControl> CreateCheckbox(string id, string text)
        {
            return new List<FluentControl>
            {
                new FluentEmpty(),
                new FluentCheckbox
                {
                    Dock = DockStyle.Fill,
                    Id = id,
                    ColumnSpan = 2,
                    Text = text,
                    Font = _fontFactory.GetNormal()
                }
            };
        }

        public IEnumerable<FluentControl> CreateDualCheckbox(string id0, string text0, string id1, string text1)
        {
            return new List<FluentControl>
            {
                new FluentEmpty(),
                new FluentTable
                {
                    Padding = Padding.Empty,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    ColumnSpan = 2,
                    Columns = new List<ColumnStyle>
                    {
                        new ColumnStyle(SizeType.Percent, 50),
                        new ColumnStyle(SizeType.Percent, 50)
                    },
                    Rows = new List<RowStyle>
                    {
                        new RowStyle(SizeType.AutoSize)
                    },
                    Controls = new List<FluentControl>
                    {
                        new FluentCheckbox
                        {
                            Dock = DockStyle.Fill,
                            Id = id0,
                            Text = text0,
                            Font = _fontFactory.GetNormal(),
                            AutoSize = true
                        },
                        new FluentCheckbox
                        {
                            Dock = DockStyle.Fill,
                            Id = id1,
                            Text = text1,
                            Font = _fontFactory.GetNormal(),
                            AutoSize = true
                        },
                    }
                }
            };
        }

        public FluentTable CreateStandardTable(string id, IEnumerable<FluentControl> controls, bool fill)
        {
            return new FluentTable
            {
                Id = id,
                AutoSize = true,
                Anchor = fill ? AnchorStyles.None : AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Dock = fill ? DockStyle.Fill : DockStyle.None,
                Columns = new List<ColumnStyle>
                {
                    new ColumnStyle(SizeType.Absolute, 145),
                    new ColumnStyle(SizeType.Percent, 100),
                    new ColumnStyle(SizeType.Absolute, 50)
                },
                Controls = controls.ToList()
            };
        }

        public IEnumerable<FluentControl> CreateBigButton(string id, string text, Action<FluentContext> press)
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
                    AfterBuild = x => GetContextDelegate(x, press),
                    MinimumSize = new Size(0, 45),
                    Font = _fontFactory.GetNormal()
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

        public IEnumerable<FluentControl> CreateFileSelect(string id, string text, bool multi)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight,
                    Font = _fontFactory.GetNormalDark()
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
                    Text = multi ? "...+" : "...",
                    AfterBuild = x => x.Blueprint.OnClick = () =>
                    {
                        var fileName = _fileDialog.OpenFile(x.Control.Text, null, multi);
                        if (fileName != null)
                            x.GetControl<TextBox>(id).Text = fileName;
                    },
                    Font = _fontFactory.GetNormal()
                }
            };
        }

        public IEnumerable<FluentControl> CreateFolderSelect(string id, string text)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight,
                    Font = _fontFactory.GetNormalDark()
                },
                new FluentTextBox
                {
                    Id = id,
                    Dock = DockStyle.Fill,
                    Font = _fontFactory.GetNormal()
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
                    },
                    Font = _fontFactory.GetNormal()
                }
            };
        }

        public IEnumerable<FluentControl> CreateSpacer(string text)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Align = ContentAlignment.MiddleRight,
                    Font = _fontFactory.GetLarge(),
                    ColumnSpan = 3,
                    Padding = new Padding(0, 8, 10, 3),
                    ForeColor = SystemColors.ActiveCaptionText,
                    BackColor = SystemColors.ActiveCaption
                }
            };
        }

        public IEnumerable<FluentControl> CreateTextEntry(string id, string text)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight,
                    Font = _fontFactory.GetNormalDark()
                },
                new FluentTextBox
                {
                    Id = id,
                    Dock = DockStyle.Fill,
                    Font = _fontFactory.GetNormal()
                },
                new FluentEmpty()
            };
        }

        public IEnumerable<FluentControl> CreateProgress(string id, string text)
        {
            return new FluentControl[]
            {
                new FluentLabel
                {
                    Id = id + "Label",
                    Text = text,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleRight,
                    Font = _fontFactory.GetNormalDark()
                },
                new FluentProgressBar
                {
                    Dock = DockStyle.Fill,
                    Id = id
                },
                new FluentLabel
                {
                    Text = "...",
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Align = ContentAlignment.MiddleLeft,
                    Font = _fontFactory.GetNormal(),
                    Id = id + "Percent"
                },
                new FluentEmpty
                {
                    Id = id + "Blank0"
                },
                new FluentLabel
                {
                    Id = id + "Message",
                    Text = "...",
                    AutoSize = true,
                    Align = ContentAlignment.MiddleLeft,
                    ColumnSpan = 2
                }
            };
        }

        public FluentControl CreateVerticalContainer(IEnumerable<FluentControl> controls)
        {
            return new FluentPanel
            {
                Controls = controls.ToList(),
                Dock = DockStyle.Fill,
            };
        }
    }
}
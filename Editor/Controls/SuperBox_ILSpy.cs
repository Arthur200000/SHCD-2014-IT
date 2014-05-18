using Qisi.Editor.Documents;
using Qisi.Editor.Documents.Elements;
using Qisi.Editor.Documents.Table;
using Qisi.Editor.Expression;
using Qisi.Editor.Properties;
using Qisi.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace Qisi.Editor.Controls
{
	[ComVisible(true)]
	public class SuperBox : Control
	{
		public delegate void MessageEventHandler(object sender, MessageEventArgs e);
		private enum MouseState
		{
			Select = 1,
			DragElement,
			DragBoundNS,
			DragBoundWE,
			AboveImage,
			Choice,
			DrawPen,
			DrawLine,
			InBlank,
			InOperate,
			None = 0
		}
		[Flags]
		private enum ToolStripMenuType
		{
			None = 1,
			General = 2,
			Table = 4,
			Matrix = 8,
			Insert = 16
		}
		private enum Bound
		{
			Top,
			Right,
			Bottom,
			Left
		}
		private const string inputableChars = "，。“”‘’、…：；！？";
		private object boundDragedObject;
		private SuperBox.Bound dragBound;
		private object dragedObject;
		private int caretIndex;
		private PointF caretLocation;
		private float virtualCaretX;
		private float caretHeight;
		private object caretContainer;
		private bool hasCaret;
		private long leftArrowTime;
		private long rightArrowTime;
		private long upArrowTime;
		private long downArrowTime;
		private ToolTip tip;
		private float mouseIndex;
		private object mouseContainer;
		private SuperBox.MouseState mouseState;
		private ContextMenuStrip contextMenuStrip1;
		private SuperBox.ToolStripMenuType toolStripMenuType;
		private List<int> selectedTextLast = new List<int>();
		private List<int> selectedTextNew = new List<int>();
		private List<object> selectedContainers = new List<object>();
		private float beginSelectIndex;
		private object beginSelectContainer;
		private bool freshed;
		private expression currentMatrix;
		private int currentMatrixItemIndex;
		private Queue<char> inputChars = new Queue<char>();
		private Queue<FType> inputExpressions = new Queue<FType>();
		private Queue<Image> inputImages = new Queue<Image>();
		private Queue<Point> inputTables = new Queue<Point>();
		private Pic_Tab selectedObject;
		private Document mainDocument;
		public event EventHandler ContentChanged;
		public event EventHandler OperateClicked;
		public event SuperBox.MessageEventHandler OperateDone;
		[Browsable(true), Category("Text"), DefaultValue(true), Description("限制输入"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DisplayName("InputLimited"), Localizable(true)]
		public bool InputLimited
		{
			get;
			set;
		}
		protected override bool CanEnableIme
		{
			get
			{
				return true;
			}
		}
		[Browsable(true), Category("Text"), DefaultValue(false), Description("只读"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DisplayName("ReadOnly"), Localizable(true)]
		public bool ReadOnly
		{
			get;
			set;
		}
		private SuperBox.ToolStripMenuType ToolStripState
		{
			get
			{
				return this.toolStripMenuType;
			}
			set
			{
				if (this.ReadOnly)
				{
					this.toolStripMenuType = SuperBox.ToolStripMenuType.None;
					this.contextMenuStrip1.Items.Clear();
					this.ContextMenuStrip = null;
				}
				else
				{
					this.toolStripMenuType = value;
					this.contextMenuStrip1.Items.Clear();
					if ((value & SuperBox.ToolStripMenuType.General) == SuperBox.ToolStripMenuType.General)
					{
						if (this.contextMenuStrip1.Items.Count != 0)
						{
							this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
						}
						this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("剪切", Resources.CutHS, new EventHandler(this.Cut)));
						this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("复制", Resources.CopyHS, new EventHandler(this.Copy)));
						this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("粘贴", Resources.PasteHS, new EventHandler(this.Paste)));
						this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("选择性粘贴", Resources.PasteHS, new EventHandler(this.SelectablePaste)));
						this.ContextMenuStrip = this.contextMenuStrip1;
					}
					if ((value & SuperBox.ToolStripMenuType.Insert) == SuperBox.ToolStripMenuType.Insert)
					{
						if (this.contextMenuStrip1.Items.Count != 0)
						{
							this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
						}
						ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("插入填空格");
						toolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("支持Enter的填空格", null, new EventHandler(this.InsertBlankEnterable)));
						toolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("忽略Enter的填空格", null, new EventHandler(this.InsertBlankEnterless)));
						this.contextMenuStrip1.Items.Add(toolStripMenuItem);
						this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("插入选项", null, new EventHandler(this.InsertOptions)));
						this.ContextMenuStrip = this.contextMenuStrip1;
					}
				}
			}
		}
		internal List<Element> Elements
		{
			get
			{
				return this.mainDocument.Elements;
			}
		}
		public SuperBox() : this(200)
		{
		}
		public SuperBox(int width)
		{
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.tip = new ToolTip();
			base.SizeChanged += new EventHandler(this.SuperBox_SizeChanged);
			base.KeyDown += new KeyEventHandler(this.SuperBox_KeyDown);
			base.KeyPress += new KeyPressEventHandler(this.SuperBox_KeyPress);
			base.MouseDown += new MouseEventHandler(this.SuperBox_MouseDown);
			base.MouseMove += new MouseEventHandler(this.SuperBox_MouseMove);
			base.MouseUp += new MouseEventHandler(this.SuperBox_MouseUp);
			base.Click += new EventHandler(this.SuperBox_Click);
			base.MouseHover += new EventHandler(this.SuperBox_MouseHover);
			this.contextMenuStrip1 = new ContextMenuStrip();
			base.TabStop = false;
			base.Width = width;
			this.DoubleBuffered = true;
			this.Font = new Font("宋体", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.caretHeight = 0f;
			this.mouseState = SuperBox.MouseState.None;
			this.hasCaret = false;
			this.mainDocument = new Document(new Padding(10, 0, 10, 10), this.Font, null, (float)base.ClientSize.Width, new PointF(0f, 0f), this.BackColor);
			this.mainDocument.ContentChanged += new EventHandler(this.mainDocument_ContentChanged);
			this.mainDocument.HeightChanged += new EventHandler(this.mainDocument_HeightChanged);
			this.mainDocument.OperateClicked += new EventHandler(this.mainDocument_OperateClicked);
			this.mainDocument.OperateDone += new EventHandler(this.mainDocument_OperateDone);
			base.Height = (int)this.mainDocument.OutHeight;
			this.Insert();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.mainDocument.Dispose();
				this.contextMenuStrip1.Dispose();
			}
			base.Dispose(disposing);
		}
		~SuperBox()
		{
			this.Dispose(false);
		}
		private void CancelSelection()
		{
			if (this.selectedObject != null)
			{
				this.selectedObject.AloneSelected = false;
				this.selectedObject = null;
			}
			this.selectedContainers = new List<object>();
			this.selectedTextLast = new List<int>();
			this.selectedTextNew = new List<int>();
		}
		private void getPrevBlank(Document document, int index, bool startFromBlank)
		{
			if (!startFromBlank)
			{
				foreach (Blank current in document.Blanks)
				{
					if (current.StartIndex + current.Count == index)
					{
						this.caretContainer = document;
						this.caretIndex = index;
						this.UpdateCaretLocation();
						return;
					}
				}
			}
			if (index > 0)
			{
				if (document.Elements[index - 1] is PictureInfo)
				{
					PictureInfo pictureInfo = document.Elements[index - 1] as PictureInfo;
					if (pictureInfo.Documents != null)
					{
						this.getPrevBlank(pictureInfo.Documents[pictureInfo.Documents.Count - 1], pictureInfo.Documents[pictureInfo.Documents.Count - 1].Elements.Count, false);
					}
					else
					{
						this.getPrevBlank(document, index - 1, false);
					}
				}
				else
				{
					if (document.Elements[index - 1] is TableInfo)
					{
						TableInfo tableInfo = document.Elements[index - 1] as TableInfo;
						if (tableInfo.Items != null)
						{
							this.getPrevBlank(tableInfo.Items[tableInfo.Items.Count - 1], tableInfo.Items[tableInfo.Items.Count - 1].Elements.Count, false);
						}
						else
						{
							this.getPrevBlank(document, index - 1, false);
						}
					}
					else
					{
						this.getPrevBlank(document, index - 1, false);
					}
				}
			}
			else
			{
				if (document.Parent != null)
				{
					if (document.Parent is PictureInfo)
					{
						PictureInfo pictureInfo = document.Parent as PictureInfo;
						int num = pictureInfo.Documents.IndexOf(document);
						if (num - 1 >= 0)
						{
							this.getPrevBlank(pictureInfo.Documents[num - 1], pictureInfo.Documents[num - 1].Elements.Count, false);
						}
						else
						{
							this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
						}
					}
					else
					{
						if (document.Parent is TableInfo)
						{
							TableInfo tableInfo = document.Parent as TableInfo;
							int num = tableInfo.Items.IndexOf(document as Cell);
							if (num - 1 >= 0)
							{
								this.getPrevBlank(tableInfo.Items[num - 1], tableInfo.Items[num - 1].Elements.Count, false);
							}
							else
							{
								this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
							}
						}
						else
						{
							this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
						}
					}
				}
			}
		}
		private void getNextBlank(Document document, int index, bool startFromBlank)
		{
			if (!startFromBlank)
			{
				foreach (Blank current in document.Blanks)
				{
					if (current.StartIndex == index)
					{
						this.caretContainer = document;
						this.caretIndex = index;
						this.UpdateCaretLocation();
						return;
					}
				}
			}
			if (index < document.Elements.Count)
			{
				if (document.Elements[index] is PictureInfo)
				{
					PictureInfo pictureInfo = document.Elements[index] as PictureInfo;
					if (pictureInfo.Documents != null)
					{
						this.getNextBlank(pictureInfo.Documents[0], 0, false);
					}
					else
					{
						this.getNextBlank(document, index + 1, false);
					}
				}
				else
				{
					if (document.Elements[index] is TableInfo)
					{
						TableInfo tableInfo = document.Elements[index] as TableInfo;
						if (tableInfo.Items != null)
						{
							this.getNextBlank(tableInfo.Items[0], 0, false);
						}
						else
						{
							this.getNextBlank(document, index + 1, false);
						}
					}
					else
					{
						this.getNextBlank(document, index + 1, false);
					}
				}
			}
			else
			{
				if (document.Parent != null)
				{
					if (document.Parent is PictureInfo)
					{
						PictureInfo pictureInfo = document.Parent as PictureInfo;
						int num = pictureInfo.Documents.IndexOf(document);
						if (num + 1 < pictureInfo.Documents.Count)
						{
							this.getNextBlank(pictureInfo.Documents[num + 1], 0, false);
						}
						else
						{
							this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
						}
					}
					else
					{
						if (document.Parent is TableInfo)
						{
							TableInfo tableInfo = document.Parent as TableInfo;
							int num = tableInfo.Items.IndexOf(document as Cell);
							if (num + 1 < tableInfo.Items.Count)
							{
								this.getNextBlank(tableInfo.Items[num + 1], 0, false);
							}
							else
							{
								this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
							}
						}
						else
						{
							this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
						}
					}
				}
			}
		}
		private void moveCaretLeft(Document document, int index)
		{
			if (document.Elements[index - 1] is CharInfo)
			{
				this.caretIndex = index - 1;
				this.UpdateCaretLocation();
			}
			else
			{
				if (document.Elements[index - 1] is ExpressionInfo)
				{
					ExpressionInfo expressionInfo = document.Elements[index - 1] as ExpressionInfo;
					containerexpression containerExpression = expressionInfo.ContainerExpression;
					this.caretContainer = containerExpression;
					this.caretIndex = containerExpression.Child.Count;
					this.UpdateCaretLocation();
					base.Invalidate();
				}
				else
				{
					if (document.Elements[index - 1] is TableInfo)
					{
						TableInfo tableInfo = document.Elements[index - 1] as TableInfo;
						this.caretContainer = tableInfo.Items[tableInfo.Items.Count - 1];
						this.caretIndex = tableInfo.Items[tableInfo.Items.Count - 1].Elements.Count;
						this.UpdateCaretLocation();
					}
					else
					{
						if (document.Elements[index - 1] is PictureInfo)
						{
							PictureInfo pictureInfo = document.Elements[index - 1] as PictureInfo;
							if (pictureInfo.Documents == null || pictureInfo.Documents.Count == 0)
							{
								this.caretIndex = index - 1;
							}
							else
							{
								this.caretContainer = pictureInfo.Documents[pictureInfo.Documents.Count - 1];
								this.caretIndex = pictureInfo.Documents[pictureInfo.Documents.Count - 1].Elements.Count;
							}
							this.UpdateCaretLocation();
						}
					}
				}
			}
		}
		private void moveCaretRight(Document document, int index)
		{
			if (document.Elements[index] is CharInfo)
			{
				this.caretIndex = index + 1;
				this.UpdateCaretLocation();
			}
			else
			{
				if (document.Elements[index] is ExpressionInfo)
				{
					ExpressionInfo expressionInfo = document.Elements[index] as ExpressionInfo;
					containerexpression containerExpression = expressionInfo.ContainerExpression;
					this.caretContainer = containerExpression;
					this.caretIndex = 0;
					this.UpdateCaretLocation();
					base.Invalidate();
				}
				else
				{
					if (document.Elements[index] is TableInfo)
					{
						TableInfo tableInfo = document.Elements[index] as TableInfo;
						this.caretContainer = tableInfo.Items[0];
						this.caretIndex = 0;
						this.UpdateCaretLocation();
					}
					else
					{
						if (document.Elements[index] is PictureInfo)
						{
							PictureInfo pictureInfo = document.Elements[index] as PictureInfo;
							if (pictureInfo.Documents == null || pictureInfo.Documents.Count == 0)
							{
								this.caretIndex = index + 1;
							}
							else
							{
								this.caretContainer = pictureInfo.Documents[0];
								this.caretIndex = 0;
							}
							this.UpdateCaretLocation();
						}
					}
				}
			}
		}
		private void findClosestElementUp(Document document, int index)
		{
			if (document.Elements[index] is PictureInfo)
			{
				PictureInfo pictureInfo = document.Elements[index] as PictureInfo;
				if (pictureInfo.Documents != null && pictureInfo.Documents.Count != 0)
				{
					Document document2 = pictureInfo.Documents[pictureInfo.Documents.Count - 1];
					for (int i = pictureInfo.Documents.Count - 2; i >= 0; i--)
					{
						if (document2.OutHeight + document2.DocLocation.Y < pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y)
						{
							document2 = pictureInfo.Documents[i];
						}
						else
						{
							if (document2.OutHeight + document2.DocLocation.Y == pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y)
							{
								float num = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
								float num2 = this.virtualCaretX - document2.DocLocation.X;
								float num3 = (num * num2 > 0f) ? Math.Abs(num + num2) : 0f;
								float num4 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X - pictureInfo.Documents[i].OutWidth;
								float num5 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X;
								float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
								if (num6 < num3)
								{
									document2 = pictureInfo.Documents[i];
								}
								else
								{
									if (num6 == num3)
									{
										if (pictureInfo.Documents[i].DocWidth < document2.DocWidth)
										{
											document2 = pictureInfo.Documents[i];
										}
									}
								}
							}
						}
					}
					Line lastLine = document2.LastLine;
					int num7 = -1;
					float num8 = float.PositiveInfinity;
					for (int i = lastLine.StartIndex + lastLine.ElementCount - 1; i >= lastLine.StartIndex; i--)
					{
						float num4 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
						float num5 = this.virtualCaretX - document.Elements[i].OutLocation.X;
						float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
						if (num6 < num8)
						{
							num8 = num6;
							num7 = i;
						}
					}
					if (num7 == -1)
					{
						this.caretContainer = document2;
						this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
						this.UpdateCaretLocation();
					}
					else
					{
						this.findClosestElementUp(document2, num7);
					}
				}
				else
				{
					this.caretContainer = document;
					if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
					{
						this.caretIndex = index + 1;
						this.UpdateCaretLocation();
					}
					else
					{
						this.caretIndex = index;
						this.UpdateCaretLocation();
					}
				}
			}
			else
			{
				if (document.Elements[index] is TableInfo)
				{
					TableInfo tableInfo = document.Elements[index] as TableInfo;
					if (tableInfo.Items != null && tableInfo.Items.Count != 0)
					{
						Document document2 = tableInfo.Items[tableInfo.Items.Count - 1];
						for (int i = tableInfo.Items.Count - 2; i >= 0; i--)
						{
							if (document2.OutHeight + document2.DocLocation.Y < tableInfo.Items[i].OutHeight + tableInfo.Items[i].DocLocation.Y)
							{
								document2 = tableInfo.Items[i];
							}
							else
							{
								if (document2.OutHeight + document2.DocLocation.Y == tableInfo.Items[i].OutHeight + tableInfo.Items[i].DocLocation.Y)
								{
									float num = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
									float num2 = this.virtualCaretX - document2.DocLocation.X;
									float num3 = (num * num2 > 0f) ? Math.Abs(num + num2) : 0f;
									float num4 = this.virtualCaretX - tableInfo.Items[i].DocLocation.X - tableInfo.Items[i].OutWidth;
									float num5 = this.virtualCaretX - tableInfo.Items[i].DocLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										document2 = tableInfo.Items[i];
									}
									else
									{
										if (num6 == num3)
										{
											if (tableInfo.Items[i].DocWidth < document2.DocWidth)
											{
												document2 = tableInfo.Items[i];
											}
										}
									}
								}
							}
						}
						Line lastLine = document2.LastLine;
						int num7 = -1;
						float num8 = float.PositiveInfinity;
						for (int i = lastLine.StartIndex + lastLine.ElementCount - 1; i >= lastLine.StartIndex; i--)
						{
							float num4 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
							float num5 = this.virtualCaretX - document.Elements[i].OutLocation.X;
							float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
							if (num6 < num8)
							{
								num8 = num6;
								num7 = i;
							}
						}
						if (num7 == -1)
						{
							this.caretContainer = document2;
							this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
							this.UpdateCaretLocation();
						}
						else
						{
							this.findClosestElementUp(document2, num7);
						}
					}
					else
					{
						this.caretContainer = document;
						if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
						{
							this.caretIndex = index + 1;
							this.UpdateCaretLocation();
						}
						else
						{
							this.caretIndex = index;
							this.UpdateCaretLocation();
						}
					}
				}
				else
				{
					if (document.Elements[index] is ExpressionInfo)
					{
						ExpressionInfo expressionInfo = document.Elements[index] as ExpressionInfo;
						this.findClosestElementUp(expressionInfo.ContainerExpression);
					}
					else
					{
						this.caretContainer = document;
						if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
						{
							this.caretIndex = index + 1;
							this.UpdateCaretLocation();
						}
						else
						{
							this.caretIndex = index;
							this.UpdateCaretLocation();
						}
					}
				}
			}
		}
		private void findClosestElementUp(lineexpression lexpr)
		{
			int num = -1;
			float num2 = float.PositiveInfinity;
			for (int i = lexpr.Child.Count - 1; i >= 0; i--)
			{
				structexpression structexpression = lexpr.Child[i];
				float num3 = this.virtualCaretX - structexpression.InputLocation.X - structexpression.Region.Width;
				float num4 = this.virtualCaretX - structexpression.InputLocation.X;
				float num5 = (num3 * num4 > 0f) ? Math.Abs(num3 + num4) : 0f;
				if (num5 < num2)
				{
					num2 = num5;
					num = i;
				}
			}
			if (num == -1)
			{
				this.caretContainer = lexpr;
				this.caretIndex = lexpr.Child.Count;
				this.UpdateCaretLocation();
			}
			else
			{
				structexpression structexpression = lexpr.Child[num];
				if (structexpression.Child != null && structexpression.Child.Count != 0)
				{
					lineexpression lineexpression = structexpression.Child[structexpression.Child.Count - 1];
					for (int i = structexpression.Child.Count - 2; i >= 0; i--)
					{
						if (lineexpression.Region.Height + lineexpression.InputLocation.Y < structexpression.Child[i].Region.Height + structexpression.Child[i].InputLocation.Y)
						{
							lineexpression = structexpression.Child[i];
						}
						else
						{
							if (lineexpression.Region.Height + lineexpression.InputLocation.Y == structexpression.Child[i].Region.Height + structexpression.Child[i].InputLocation.Y)
							{
								float num6 = this.virtualCaretX - lineexpression.InputLocation.X - lineexpression.Region.Width;
								float num7 = this.virtualCaretX - lineexpression.InputLocation.X;
								float num8 = (num6 * num7 > 0f) ? Math.Abs(num6 + num7) : 0f;
								float num3 = this.virtualCaretX - structexpression.Child[i].InputLocation.X - structexpression.Child[i].Region.Width;
								float num4 = this.virtualCaretX - structexpression.Child[i].InputLocation.X;
								float num5 = (num3 * num4 > 0f) ? Math.Abs(num3 + num4) : 0f;
								if (num5 < num8)
								{
									lineexpression = structexpression.Child[i];
								}
								else
								{
									if (num5 == num8)
									{
										if (structexpression.Child[i].Region.Width < lineexpression.Region.Width)
										{
											lineexpression = structexpression.Child[i];
										}
									}
								}
							}
						}
					}
					this.findClosestElementUp(lineexpression);
				}
				else
				{
					this.caretContainer = lexpr;
					if ((double)((this.virtualCaretX - structexpression.InputLocation.X) / structexpression.Region.Width) > 0.5)
					{
						this.caretIndex = num + 1;
						this.UpdateCaretLocation();
					}
					else
					{
						this.caretIndex = num;
						this.UpdateCaretLocation();
					}
				}
			}
		}
		private void findUpperLine(Document document)
		{
			if (document.Parent != null)
			{
				if (document.Parent is TableInfo)
				{
					TableInfo tableInfo = document.Parent as TableInfo;
					if (tableInfo.Items != null && tableInfo.Items.Count > 1)
					{
						Document document2 = null;
						int num = tableInfo.Items.IndexOf(document as Cell);
						if (num - tableInfo.TableSize.Y >= 0)
						{
							document2 = tableInfo.Items[num - tableInfo.TableSize.Y];
						}
						if (document2 != null)
						{
							Line lastLine = document2.LastLine;
							int num2 = -1;
							float num3 = float.PositiveInfinity;
							for (int i = lastLine.StartIndex + lastLine.ElementCount - 1; i >= lastLine.StartIndex; i--)
							{
								float num4 = this.virtualCaretX - document2.Elements[i].OutLocation.X - document2.Elements[i].OutSize.Width;
								float num5 = this.virtualCaretX - document2.Elements[i].OutLocation.X;
								float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
								if (num6 < num3)
								{
									num3 = num6;
									num2 = i;
								}
							}
							if (num2 == -1)
							{
								this.caretContainer = document2;
								this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
								this.UpdateCaretLocation();
							}
							else
							{
								this.findClosestElementUp(document2, num2);
							}
						}
						else
						{
							Document documentContainer = tableInfo.DocumentContainer;
							Line lineContainer = tableInfo.LineContainer;
							int num7 = documentContainer.Lines.IndexOf(lineContainer);
							if (num7 > 0)
							{
								Line line = documentContainer.Lines[num7 - 1];
								int num8 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
								{
									if (!documentContainer.Elements[i].InBlank)
									{
										float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
										float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
										float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
										if (num6 < num3)
										{
											num3 = num6;
											num8 = i;
										}
									}
								}
								if (num8 != -1)
								{
									this.findClosestElementUp(documentContainer, num8);
								}
							}
							else
							{
								this.findUpperLine(documentContainer);
							}
						}
					}
					else
					{
						Document documentContainer = tableInfo.DocumentContainer;
						Line lineContainer = tableInfo.LineContainer;
						int num7 = documentContainer.Lines.IndexOf(lineContainer);
						if (num7 > 0)
						{
							Line line = documentContainer.Lines[num7 - 1];
							int num8 = -1;
							float num3 = float.PositiveInfinity;
							for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
							{
								if (!documentContainer.Elements[i].InBlank)
								{
									float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
									float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										num3 = num6;
										num8 = i;
									}
								}
							}
							if (num8 != -1)
							{
								this.findClosestElementUp(documentContainer, num8);
							}
						}
						else
						{
							this.findUpperLine(documentContainer);
						}
					}
				}
				else
				{
					if (document.Parent is PictureInfo)
					{
						PictureInfo pictureInfo = document.Parent as PictureInfo;
						if (pictureInfo.Documents != null && pictureInfo.Documents.Count > 1)
						{
							Document document2 = null;
							float num9 = float.NegativeInfinity;
							for (int i = pictureInfo.Documents.Count - 1; i >= 0; i--)
							{
								if (document != pictureInfo.Documents[i])
								{
									if (document.OutHeight + document.DocLocation.Y > pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y)
									{
										if (num9 < pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y)
										{
											document2 = pictureInfo.Documents[i];
											num9 = pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y;
										}
										else
										{
											if (num9 == pictureInfo.Documents[i].OutHeight + pictureInfo.Documents[i].DocLocation.Y)
											{
												float num10 = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
												float num11 = this.virtualCaretX - document2.DocLocation.X;
												float num12 = (num10 * num11 > 0f) ? Math.Abs(num10 + num11) : 0f;
												float num4 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X - pictureInfo.Documents[i].OutWidth;
												float num5 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X;
												float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
												if (num6 < num12)
												{
													document2 = pictureInfo.Documents[i];
												}
												else
												{
													if (num6 == num12)
													{
														if (pictureInfo.Documents[i].DocWidth < document2.DocWidth)
														{
															document2 = pictureInfo.Documents[i];
														}
													}
												}
											}
										}
									}
								}
							}
							if (document2 != null)
							{
								Line lastLine = document2.LastLine;
								int num2 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = lastLine.StartIndex + lastLine.ElementCount - 1; i >= lastLine.StartIndex; i--)
								{
									float num4 = this.virtualCaretX - document2.Elements[i].OutLocation.X - document2.Elements[i].OutSize.Width;
									float num5 = this.virtualCaretX - document2.Elements[i].OutLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										num3 = num6;
										num2 = i;
									}
								}
								if (num2 == -1)
								{
									this.caretContainer = document2;
									this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
									this.UpdateCaretLocation();
								}
								else
								{
									this.findClosestElementUp(document2, num2);
								}
							}
							else
							{
								Document documentContainer = pictureInfo.DocumentContainer;
								Line lineContainer = pictureInfo.LineContainer;
								int num7 = documentContainer.Lines.IndexOf(lineContainer);
								if (num7 > 0)
								{
									Line line = documentContainer.Lines[num7 - 1];
									int num8 = -1;
									float num3 = float.PositiveInfinity;
									for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
									{
										if (!documentContainer.Elements[i].InBlank)
										{
											float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
											float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
											float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
											if (num6 < num3)
											{
												num3 = num6;
												num8 = i;
											}
										}
									}
									if (num8 != -1)
									{
										this.findClosestElementUp(documentContainer, num8);
									}
								}
								else
								{
									this.findUpperLine(documentContainer);
								}
							}
						}
						else
						{
							Document documentContainer = pictureInfo.DocumentContainer;
							Line lineContainer = pictureInfo.LineContainer;
							int num7 = documentContainer.Lines.IndexOf(lineContainer);
							if (num7 > 0)
							{
								Line line = documentContainer.Lines[num7 - 1];
								int num8 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
								{
									if (!documentContainer.Elements[i].InBlank)
									{
										float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
										float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
										float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
										if (num6 < num3)
										{
											num3 = num6;
											num8 = i;
										}
									}
								}
								if (num8 != -1)
								{
									this.findClosestElementUp(documentContainer, num8);
								}
							}
							else
							{
								this.findUpperLine(documentContainer);
							}
						}
					}
				}
			}
		}
		private void findClosestElementDown(Document document, int index)
		{
			if (document.Elements[index] is PictureInfo)
			{
				PictureInfo pictureInfo = document.Elements[index] as PictureInfo;
				if (pictureInfo.Documents != null && pictureInfo.Documents.Count != 0)
				{
					Document document2 = pictureInfo.Documents[0];
					for (int i = 1; i < pictureInfo.Documents.Count; i++)
					{
						if (document2.DocLocation.Y > pictureInfo.Documents[i].DocLocation.Y)
						{
							document2 = pictureInfo.Documents[i];
						}
						else
						{
							if (document2.DocLocation.Y == pictureInfo.Documents[i].DocLocation.Y)
							{
								float num = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
								float num2 = this.virtualCaretX - document2.DocLocation.X;
								float num3 = (num * num2 > 0f) ? Math.Abs(num + num2) : 0f;
								float num4 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X - pictureInfo.Documents[i].OutWidth;
								float num5 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X;
								float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
								if (num6 < num3)
								{
									document2 = pictureInfo.Documents[i];
								}
								else
								{
									if (num6 == num3)
									{
										if (pictureInfo.Documents[i].DocWidth < document2.DocWidth)
										{
											document2 = pictureInfo.Documents[i];
										}
									}
								}
							}
						}
					}
					Line line = document2.Lines[0];
					int num7 = -1;
					float num8 = float.PositiveInfinity;
					for (int i = line.StartIndex; i < line.StartIndex + line.ElementCount; i++)
					{
						float num4 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
						float num5 = this.virtualCaretX - document.Elements[i].OutLocation.X;
						float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
						if (num6 < num8)
						{
							num8 = num6;
							num7 = i;
						}
					}
					if (num7 == -1)
					{
						this.caretContainer = document2;
						this.caretIndex = line.StartIndex + line.ElementCount;
						this.UpdateCaretLocation();
					}
					else
					{
						this.findClosestElementDown(document2, num7);
					}
				}
				else
				{
					this.caretContainer = document;
					if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
					{
						this.caretIndex = index + 1;
						this.UpdateCaretLocation();
					}
					else
					{
						this.caretIndex = index;
						this.UpdateCaretLocation();
					}
				}
			}
			else
			{
				if (document.Elements[index] is TableInfo)
				{
					TableInfo tableInfo = document.Elements[index] as TableInfo;
					if (tableInfo.Items != null && tableInfo.Items.Count != 0)
					{
						Document document2 = tableInfo.Items[tableInfo.Items.Count - 1];
						for (int i = 1; i < tableInfo.Items.Count; i++)
						{
							if (document2.DocLocation.Y > tableInfo.Items[i].DocLocation.Y)
							{
								document2 = tableInfo.Items[i];
							}
							else
							{
								if (document2.DocLocation.Y == tableInfo.Items[i].DocLocation.Y)
								{
									float num = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
									float num2 = this.virtualCaretX - document2.DocLocation.X;
									float num3 = (num * num2 > 0f) ? Math.Abs(num + num2) : 0f;
									float num4 = this.virtualCaretX - tableInfo.Items[i].DocLocation.X - tableInfo.Items[i].OutWidth;
									float num5 = this.virtualCaretX - tableInfo.Items[i].DocLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										document2 = tableInfo.Items[i];
									}
									else
									{
										if (num6 == num3)
										{
											if (tableInfo.Items[i].DocWidth < document2.DocWidth)
											{
												document2 = tableInfo.Items[i];
											}
										}
									}
								}
							}
						}
						Line line = document2.Lines[0];
						int num7 = -1;
						float num8 = float.PositiveInfinity;
						for (int i = line.StartIndex; i < line.StartIndex + line.ElementCount; i++)
						{
							float num4 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
							float num5 = this.virtualCaretX - document.Elements[i].OutLocation.X;
							float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
							if (num6 < num8)
							{
								num8 = num6;
								num7 = i;
							}
						}
						if (num7 == -1)
						{
							this.caretContainer = document2;
							this.caretIndex = line.StartIndex + line.ElementCount;
							this.UpdateCaretLocation();
						}
						else
						{
							this.findClosestElementDown(document2, num7);
						}
					}
					else
					{
						this.caretContainer = document;
						if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
						{
							this.caretIndex = index + 1;
							this.UpdateCaretLocation();
						}
						else
						{
							this.caretIndex = index;
							this.UpdateCaretLocation();
						}
					}
				}
				else
				{
					if (document.Elements[index] is ExpressionInfo)
					{
						ExpressionInfo expressionInfo = document.Elements[index] as ExpressionInfo;
						this.findClosestElementDown(expressionInfo.ContainerExpression);
					}
					else
					{
						this.caretContainer = document;
						if ((double)((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
						{
							this.caretIndex = index + 1;
							this.UpdateCaretLocation();
						}
						else
						{
							this.caretIndex = index;
							this.UpdateCaretLocation();
						}
					}
				}
			}
		}
		private void findClosestElementDown(lineexpression lexpr)
		{
			int num = -1;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < lexpr.Child.Count; i++)
			{
				structexpression structexpression = lexpr.Child[i];
				float num3 = this.virtualCaretX - structexpression.InputLocation.X - structexpression.Region.Width;
				float num4 = this.virtualCaretX - structexpression.InputLocation.X;
				float num5 = (num3 * num4 > 0f) ? Math.Abs(num3 + num4) : 0f;
				if (num5 < num2)
				{
					num2 = num5;
					num = i;
				}
			}
			if (num == -1)
			{
				this.caretContainer = lexpr;
				this.caretIndex = lexpr.Child.Count;
				this.UpdateCaretLocation();
			}
			else
			{
				structexpression structexpression = lexpr.Child[num];
				if (structexpression.Child != null && structexpression.Child.Count != 0)
				{
					lineexpression lineexpression = structexpression.Child[0];
					for (int i = 1; i < structexpression.Child.Count; i++)
					{
						if (lineexpression.InputLocation.Y > structexpression.Child[i].InputLocation.Y)
						{
							lineexpression = structexpression.Child[i];
						}
						else
						{
							if (lineexpression.InputLocation.Y == structexpression.Child[i].InputLocation.Y)
							{
								float num6 = this.virtualCaretX - lineexpression.InputLocation.X - lineexpression.Region.Width;
								float num7 = this.virtualCaretX - lineexpression.InputLocation.X;
								float num8 = (num6 * num7 > 0f) ? Math.Abs(num6 + num7) : 0f;
								float num3 = this.virtualCaretX - structexpression.Child[i].InputLocation.X - structexpression.Child[i].Region.Width;
								float num4 = this.virtualCaretX - structexpression.Child[i].InputLocation.X;
								float num5 = (num3 * num4 > 0f) ? Math.Abs(num3 + num4) : 0f;
								if (num5 < num8)
								{
									lineexpression = structexpression.Child[i];
								}
								else
								{
									if (num5 == num8)
									{
										if (structexpression.Child[i].Region.Width < lineexpression.Region.Width)
										{
											lineexpression = structexpression.Child[i];
										}
									}
								}
							}
						}
					}
					this.findClosestElementDown(lineexpression);
				}
				else
				{
					this.caretContainer = lexpr;
					if ((double)((this.virtualCaretX - structexpression.InputLocation.X) / structexpression.Region.Width) > 0.5)
					{
						this.caretIndex = num + 1;
						this.UpdateCaretLocation();
					}
					else
					{
						this.caretIndex = num;
						this.UpdateCaretLocation();
					}
				}
			}
		}
		private void findDownLine(Document document)
		{
			if (document.Parent != null)
			{
				if (document.Parent is TableInfo)
				{
					TableInfo tableInfo = document.Parent as TableInfo;
					if (tableInfo.Items != null && tableInfo.Items.Count > 1)
					{
						Document document2 = null;
						int num = tableInfo.Items.IndexOf(document as Cell);
						if (num + tableInfo.TableSize.Y < tableInfo.Items.Count)
						{
							document2 = tableInfo.Items[num + tableInfo.TableSize.Y];
						}
						if (document2 != null)
						{
							Line line = document2.Lines[0];
							int num2 = -1;
							float num3 = float.PositiveInfinity;
							for (int i = line.StartIndex; i < line.StartIndex + line.ElementCount; i++)
							{
								float num4 = this.virtualCaretX - document2.Elements[i].OutLocation.X - document2.Elements[i].OutSize.Width;
								float num5 = this.virtualCaretX - document2.Elements[i].OutLocation.X;
								float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
								if (num6 < num3)
								{
									num3 = num6;
									num2 = i;
								}
							}
							if (num2 == -1)
							{
								this.caretContainer = document2;
								this.caretIndex = line.StartIndex + line.ElementCount;
								this.UpdateCaretLocation();
							}
							else
							{
								this.findClosestElementDown(document2, num2);
							}
						}
						else
						{
							Document documentContainer = tableInfo.DocumentContainer;
							Line lineContainer = tableInfo.LineContainer;
							int num7 = documentContainer.Lines.IndexOf(lineContainer);
							if (num7 < documentContainer.Lines.Count - 1)
							{
								Line line2 = documentContainer.Lines[num7 + 1];
								int num8 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.StartIndex + line2.StartIndex; i++)
								{
									if (!documentContainer.Elements[i].InBlank)
									{
										float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
										float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
										float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
										if (num6 < num3)
										{
											num3 = num6;
											num8 = i;
										}
									}
								}
								if (num8 != -1)
								{
									this.findClosestElementDown(documentContainer, num8);
								}
							}
							else
							{
								this.findDownLine(documentContainer);
							}
						}
					}
					else
					{
						Document documentContainer = tableInfo.DocumentContainer;
						Line lineContainer = tableInfo.LineContainer;
						int num7 = documentContainer.Lines.IndexOf(lineContainer);
						if (num7 < documentContainer.Lines.Count - 1)
						{
							Line line2 = documentContainer.Lines[num7 + 1];
							int num8 = -1;
							float num3 = float.PositiveInfinity;
							for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.StartIndex + line2.StartIndex; i++)
							{
								if (!documentContainer.Elements[i].InBlank)
								{
									float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
									float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										num3 = num6;
										num8 = i;
									}
								}
							}
							if (num8 != -1)
							{
								this.findClosestElementDown(documentContainer, num8);
							}
						}
						else
						{
							this.findDownLine(documentContainer);
						}
					}
				}
				else
				{
					if (document.Parent is PictureInfo)
					{
						PictureInfo pictureInfo = document.Parent as PictureInfo;
						if (pictureInfo.Documents != null && pictureInfo.Documents.Count > 1)
						{
							Document document2 = null;
							float num9 = float.PositiveInfinity;
							for (int i = 0; i < pictureInfo.Documents.Count; i++)
							{
								if (document != pictureInfo.Documents[i])
								{
									if (document.DocLocation.Y < pictureInfo.Documents[i].DocLocation.Y)
									{
										if (num9 > pictureInfo.Documents[i].DocLocation.Y)
										{
											document2 = pictureInfo.Documents[i];
											num9 = pictureInfo.Documents[i].DocLocation.Y;
										}
										else
										{
											if (num9 == pictureInfo.Documents[i].DocLocation.Y)
											{
												float num10 = this.virtualCaretX - document2.DocLocation.X - document2.OutWidth;
												float num11 = this.virtualCaretX - document2.DocLocation.X;
												float num12 = (num10 * num11 > 0f) ? Math.Abs(num10 + num11) : 0f;
												float num4 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X - pictureInfo.Documents[i].OutWidth;
												float num5 = this.virtualCaretX - pictureInfo.Documents[i].DocLocation.X;
												float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
												if (num6 < num12)
												{
													document2 = pictureInfo.Documents[i];
												}
												else
												{
													if (num6 == num12)
													{
														if (pictureInfo.Documents[i].DocWidth < document2.DocWidth)
														{
															document2 = pictureInfo.Documents[i];
														}
													}
												}
											}
										}
									}
								}
							}
							if (document2 != null)
							{
								Line line = document2.Lines[0];
								int num2 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = line.StartIndex; i < line.StartIndex + line.ElementCount; i++)
								{
									float num4 = this.virtualCaretX - document2.Elements[i].OutLocation.X - document2.Elements[i].OutSize.Width;
									float num5 = this.virtualCaretX - document2.Elements[i].OutLocation.X;
									float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
									if (num6 < num3)
									{
										num3 = num6;
										num2 = i;
									}
								}
								if (num2 == -1)
								{
									this.caretContainer = document2;
									this.caretIndex = line.StartIndex + line.ElementCount;
									this.UpdateCaretLocation();
								}
								else
								{
									this.findClosestElementDown(document2, num2);
								}
							}
							else
							{
								Document documentContainer = pictureInfo.DocumentContainer;
								Line lineContainer = pictureInfo.LineContainer;
								int num7 = documentContainer.Lines.IndexOf(lineContainer);
								if (num7 < documentContainer.Lines.Count - 1)
								{
									Line line2 = documentContainer.Lines[num7 + 1];
									int num8 = -1;
									float num3 = float.PositiveInfinity;
									for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.StartIndex + line2.ElementCount; i++)
									{
										if (!documentContainer.Elements[i].InBlank)
										{
											float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
											float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
											float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
											if (num6 < num3)
											{
												num3 = num6;
												num8 = i;
											}
										}
									}
									if (num8 != -1)
									{
										this.findClosestElementDown(documentContainer, num8);
									}
								}
								else
								{
									this.findDownLine(documentContainer);
								}
							}
						}
						else
						{
							Document documentContainer = pictureInfo.DocumentContainer;
							Line lineContainer = pictureInfo.LineContainer;
							int num7 = documentContainer.Lines.IndexOf(lineContainer);
							if (num7 < documentContainer.Lines.Count - 1)
							{
								Line line2 = documentContainer.Lines[num7 + 1];
								int num8 = -1;
								float num3 = float.PositiveInfinity;
								for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.StartIndex + line2.ElementCount; i++)
								{
									if (!documentContainer.Elements[i].InBlank)
									{
										float num4 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X - documentContainer.Elements[i].OutSize.Width;
										float num5 = this.virtualCaretX - documentContainer.Elements[i].OutLocation.X;
										float num6 = (num4 * num5 > 0f) ? Math.Abs(num4 + num5) : 0f;
										if (num6 < num3)
										{
											num3 = num6;
											num8 = i;
										}
									}
								}
								if (num8 != -1)
								{
									this.findClosestElementDown(documentContainer, num8);
								}
							}
							else
							{
								this.findDownLine(documentContainer);
							}
						}
					}
				}
			}
		}
		private void SuperBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
			if (this.InputLimited)
			{
				if ((e.KeyChar < '一' || e.KeyChar > '龥') && (e.KeyChar < ' ' || e.KeyChar > '~') && !"，。“”‘’、…：；！？".Contains(e.KeyChar.ToString()) && e.KeyChar != '\b' && e.KeyChar != '\n' && e.KeyChar != '\r')
				{
					return;
				}
			}
			else
			{
				if (e.KeyChar < ' ' && e.KeyChar != '\b' && e.KeyChar != '\n' && e.KeyChar != '\r')
				{
					return;
				}
			}
			if (!(this.caretContainer is lineexpression) || e.KeyChar != '\r')
			{
				this.inputChars.Enqueue(e.KeyChar);
				this.Insert();
			}
		}
		private void SuperBox_KeyDown(object sender, KeyEventArgs e)
		{
			IntPtr hIMC = NativeMethods.ImmGetContext(base.Handle);
			NativeMethods.COMPOSITIONFORM cOMPOSITIONFORM = default(NativeMethods.COMPOSITIONFORM);
			cOMPOSITIONFORM.dwStyle = 2;
			cOMPOSITIONFORM.ptCurrentPos = default(Point);
			cOMPOSITIONFORM.ptCurrentPos.X = (int)this.caretLocation.X;
			cOMPOSITIONFORM.ptCurrentPos.Y = (int)this.caretLocation.Y;
			int num = NativeMethods.ImmSetCompositionWindow(hIMC, ref cOMPOSITIONFORM);
		}
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool result;
			if (keyData == Keys.Tab)
			{
				if (this.ReadOnly)
				{
					Document document = null;
					if (this.caretContainer is Document)
					{
						document = (this.caretContainer as Document);
					}
					else
					{
						if (this.caretContainer is lineexpression)
						{
							lineexpression lineexpression = this.caretContainer as lineexpression;
							while (!(lineexpression is containerexpression) && lineexpression != null)
							{
								lineexpression = lineexpression.ParentExpression.ParentExpression;
							}
							if (lineexpression != null && lineexpression is containerexpression)
							{
								document = (lineexpression as containerexpression).Info.DocumentContainer;
							}
						}
					}
					if (document != null)
					{
						Blank blank = null;
						foreach (Blank current in document.Blanks)
						{
							if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
							{
								blank = current;
								break;
							}
						}
						if (blank != null)
						{
							Blank blank2 = null;
							int num = 2147483647;
							foreach (Blank current in document.Blanks)
							{
								if (current.StartIndex > blank.StartIndex + blank.Count && current.StartIndex < num)
								{
									blank2 = current;
									num = current.StartIndex;
								}
							}
							if (blank2 != null)
							{
								this.caretContainer = document;
								this.caretIndex = blank2.StartIndex;
								this.UpdateCaretLocation();
							}
						}
					}
				}
				result = true;
			}
			else
			{
				if (keyData == (Keys.LButton | Keys.Back | Keys.Shift))
				{
					if (this.ReadOnly)
					{
						Document document = null;
						if (this.caretContainer is Document)
						{
							document = (this.caretContainer as Document);
						}
						else
						{
							if (this.caretContainer is lineexpression)
							{
								lineexpression lineexpression = this.caretContainer as lineexpression;
								while (!(lineexpression is containerexpression) && lineexpression != null)
								{
									lineexpression = lineexpression.ParentExpression.ParentExpression;
								}
								if (lineexpression != null && lineexpression is containerexpression)
								{
									document = (lineexpression as containerexpression).Info.DocumentContainer;
								}
							}
						}
						if (document != null)
						{
							Blank blank = null;
							foreach (Blank current in document.Blanks)
							{
								if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
								{
									blank = current;
									break;
								}
							}
							if (blank != null)
							{
								Blank blank3 = null;
								int num2 = -2147483648;
								foreach (Blank current in document.Blanks)
								{
									if (current.StartIndex + current.Count < blank.StartIndex && current.StartIndex + current.Count > num2)
									{
										blank3 = current;
										num2 = current.StartIndex + current.Count;
									}
								}
								if (blank3 != null)
								{
									this.caretContainer = document;
									this.caretIndex = blank3.StartIndex;
									this.UpdateCaretLocation();
								}
							}
						}
					}
					result = true;
				}
				else
				{
					if (keyData == Keys.Delete)
					{
						this.inputChars.Enqueue('\u007f');
						this.Insert();
						result = true;
					}
					else
					{
						if (keyData == Keys.Left)
						{
							long num3 = DateTime.Now.Ticks;
							if (this.caretContainer is Document)
							{
								Document document = this.caretContainer as Document;
								if (this.ReadOnly)
								{
									bool flag = false;
									foreach (Blank current in document.Blanks)
									{
										if (current.StartIndex == this.caretIndex)
										{
											flag = true;
											break;
										}
									}
									if (flag)
									{
										if (num3 - this.leftArrowTime < 1500000L)
										{
											this.getPrevBlank(document, this.caretIndex, true);
											num3 = 0L;
										}
									}
									else
									{
										this.moveCaretLeft(document, this.caretIndex);
									}
								}
								else
								{
									if (this.caretIndex > 0)
									{
										bool flag2 = false;
										Blank blank4 = null;
										foreach (Blank current in document.Blanks)
										{
											if (current.StartIndex + current.Count == this.caretIndex)
											{
												flag2 = true;
												blank4 = current;
												break;
											}
										}
										if (flag2)
										{
											this.caretIndex = blank4.StartIndex;
											this.UpdateCaretLocation();
										}
										else
										{
											this.moveCaretLeft(document, this.caretIndex);
										}
									}
									else
									{
										if (document.Parent != null)
										{
											if (document.Parent is TableInfo)
											{
												TableInfo tableInfo = document.Parent as TableInfo;
												int num4 = tableInfo.Items.IndexOf(document as Cell);
												if (num4 > 0)
												{
													this.caretContainer = tableInfo.Items[num4 - 1];
													this.caretIndex = tableInfo.Items[num4 - 1].Elements.Count;
													this.UpdateCaretLocation();
												}
												else
												{
													Document documentContainer = tableInfo.DocumentContainer;
													this.caretContainer = documentContainer;
													this.caretIndex = tableInfo.Index;
													this.UpdateCaretLocation();
												}
											}
											else
											{
												if (document.Parent is PictureInfo)
												{
													PictureInfo pictureInfo = document.Parent as PictureInfo;
													int num4 = pictureInfo.Documents.IndexOf(document as Cell);
													if (num4 > 0)
													{
														this.caretContainer = pictureInfo.Documents[num4 - 1];
														this.caretIndex = pictureInfo.Documents[num4 - 1].Elements.Count;
														this.UpdateCaretLocation();
													}
													else
													{
														Document documentContainer = pictureInfo.DocumentContainer;
														this.caretContainer = documentContainer;
														this.caretIndex = pictureInfo.Index;
														this.UpdateCaretLocation();
													}
												}
											}
										}
									}
								}
							}
							else
							{
								if (this.caretContainer is lineexpression)
								{
									lineexpression lineexpression = this.caretContainer as lineexpression;
									if (this.caretIndex > 0)
									{
										if (lineexpression.Child[this.caretIndex - 1].Child != null)
										{
											lineexpression lineexpression2 = lineexpression.Child[this.caretIndex - 1].Child[lineexpression.Child[this.caretIndex - 1].Child.Count - 1];
											this.caretContainer = lineexpression2;
											this.caretIndex = lineexpression2.Child.Count;
										}
										else
										{
											this.caretIndex--;
										}
										this.UpdateCaretLocation();
									}
									else
									{
										if (lineexpression is containerexpression)
										{
											containerexpression containerexpression = lineexpression as containerexpression;
											ExpressionInfo info = containerexpression.Info;
											this.caretContainer = info.DocumentContainer;
											this.caretIndex = info.Index;
										}
										else
										{
											int num5 = lineexpression.ParentExpression.Child.IndexOf(lineexpression);
											if (num5 > 0)
											{
												this.caretContainer = lineexpression.ParentExpression.Child[num5 - 1];
												lineexpression lineexpression2 = this.caretContainer as lineexpression;
												this.caretIndex = lineexpression2.Child.Count;
											}
											else
											{
												structexpression parentExpression = lineexpression.ParentExpression;
												lineexpression lineexpression2 = parentExpression.ParentExpression;
												this.caretContainer = lineexpression2;
												this.caretIndex = lineexpression2.Child.IndexOf(parentExpression);
											}
										}
										this.UpdateCaretLocation();
									}
									base.Invalidate();
								}
							}
							this.virtualCaretX = this.caretLocation.X;
							this.leftArrowTime = num3;
							result = true;
						}
						else
						{
							if (keyData == Keys.Right)
							{
								long num3 = DateTime.Now.Ticks;
								if (this.caretContainer is Document)
								{
									Document document = this.caretContainer as Document;
									if (this.ReadOnly)
									{
										bool flag2 = false;
										foreach (Blank current in document.Blanks)
										{
											if (current.StartIndex + current.Count == this.caretIndex)
											{
												flag2 = true;
												break;
											}
										}
										if (flag2)
										{
											if (num3 - this.rightArrowTime < 1500000L)
											{
												this.getNextBlank(document, this.caretIndex, true);
												num3 = 0L;
											}
										}
										else
										{
											this.moveCaretRight(document, this.caretIndex);
										}
									}
									else
									{
										if (this.caretIndex < document.Elements.Count)
										{
											bool flag = false;
											Blank blank4 = null;
											foreach (Blank current in document.Blanks)
											{
												if (current.StartIndex == this.caretIndex)
												{
													flag = true;
													blank4 = current;
													break;
												}
											}
											if (flag)
											{
												this.caretIndex = blank4.StartIndex + blank4.Count;
												this.UpdateCaretLocation();
											}
											else
											{
												this.moveCaretRight(document, this.caretIndex);
											}
										}
										else
										{
											if (document.Parent != null)
											{
												if (document.Parent is TableInfo)
												{
													TableInfo tableInfo = document.Parent as TableInfo;
													int num4 = tableInfo.Items.IndexOf(document as Cell);
													if (num4 < tableInfo.Items.Count - 1)
													{
														this.caretContainer = tableInfo.Items[num4 + 1];
														this.caretIndex = 0;
														this.UpdateCaretLocation();
													}
													else
													{
														Document documentContainer = tableInfo.DocumentContainer;
														this.caretContainer = documentContainer;
														this.caretIndex = tableInfo.Index + 1;
														this.UpdateCaretLocation();
													}
												}
												else
												{
													if (document.Parent is PictureInfo)
													{
														PictureInfo pictureInfo = document.Parent as PictureInfo;
														int num4 = pictureInfo.Documents.IndexOf(document as Cell);
														if (num4 < pictureInfo.Documents.Count - 1)
														{
															this.caretContainer = pictureInfo.Documents[num4 + 1];
															this.caretIndex = 0;
															this.UpdateCaretLocation();
														}
														else
														{
															Document documentContainer = pictureInfo.DocumentContainer;
															this.caretContainer = documentContainer;
															this.caretIndex = pictureInfo.Index + 1;
															this.UpdateCaretLocation();
														}
													}
												}
											}
										}
									}
								}
								else
								{
									if (this.caretContainer is lineexpression)
									{
										lineexpression lineexpression = this.caretContainer as lineexpression;
										if (this.caretIndex < lineexpression.Child.Count)
										{
											if (lineexpression.Child[this.caretIndex].Child != null)
											{
												lineexpression lineexpression2 = lineexpression.Child[this.caretIndex].Child[0];
												this.caretContainer = lineexpression2;
												this.caretIndex = 0;
											}
											else
											{
												this.caretIndex++;
											}
											this.UpdateCaretLocation();
										}
										else
										{
											if (lineexpression is containerexpression)
											{
												containerexpression containerexpression = lineexpression as containerexpression;
												ExpressionInfo info = containerexpression.Info;
												this.caretContainer = info.DocumentContainer;
												this.caretIndex = info.Index + 1;
											}
											else
											{
												int num5 = lineexpression.ParentExpression.Child.IndexOf(lineexpression);
												if (num5 < lineexpression.ParentExpression.Child.Count - 1)
												{
													this.caretContainer = lineexpression.ParentExpression.Child[num5 + 1];
													lineexpression lineexpression2 = this.caretContainer as lineexpression;
													this.caretIndex = 0;
												}
												else
												{
													structexpression parentExpression = lineexpression.ParentExpression;
													lineexpression lineexpression2 = parentExpression.ParentExpression;
													this.caretContainer = lineexpression2;
													this.caretIndex = lineexpression2.Child.IndexOf(parentExpression) + 1;
												}
											}
											this.UpdateCaretLocation();
										}
									}
								}
								this.virtualCaretX = this.caretLocation.X;
								this.rightArrowTime = num3;
								result = true;
							}
							else
							{
								if (keyData == Keys.Up)
								{
									if (this.caretContainer is Document)
									{
										Document document = this.caretContainer as Document;
										Line item = (document.Elements.Count == 0 || this.caretIndex == document.Elements.Count) ? document.LastLine : document.Elements[this.caretIndex].LineContainer;
										int num6 = document.Lines.IndexOf(item);
										if (this.ReadOnly)
										{
											Blank blank = null;
											foreach (Blank current in document.Blanks)
											{
												if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
												{
													blank = current;
													break;
												}
											}
											if (blank != null && num6 > 0)
											{
												Line line = document.Lines[num6 - 1];
												int num7 = -1;
												float num8 = float.PositiveInfinity;
												for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= Math.Max(blank.StartIndex, line.StartIndex); i--)
												{
													float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
													float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
													float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
													if (num11 < num8)
													{
														num8 = num11;
														num7 = i;
													}
												}
												if (num7 != -1)
												{
													this.findClosestElementUp(document, num7);
												}
											}
										}
										else
										{
											if (num6 > 0)
											{
												Line line = document.Lines[num6 - 1];
												int num7 = -1;
												float num8 = float.PositiveInfinity;
												for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
												{
													if (!document.Elements[i].InBlank)
													{
														float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
														float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
														float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
														if (num11 < num8)
														{
															num8 = num11;
															num7 = i;
														}
													}
												}
												if (num7 != -1)
												{
													this.findClosestElementUp(document, num7);
												}
											}
											else
											{
												this.findUpperLine(document);
											}
										}
									}
									else
									{
										if (this.caretContainer is lineexpression)
										{
											lineexpression lineexpression = this.caretContainer as lineexpression;
											if (lineexpression.UpLineExpression != null)
											{
												this.findClosestElementUp(lineexpression.UpLineExpression);
											}
											else
											{
												while (!(lineexpression is containerexpression) && lineexpression != null)
												{
													lineexpression = lineexpression.ParentExpression.ParentExpression;
												}
												if (lineexpression != null)
												{
													ExpressionInfo info = (lineexpression as containerexpression).Info;
													Document document = info.DocumentContainer;
													Line item = info.LineContainer;
													int num6 = document.Lines.IndexOf(item);
													if (this.ReadOnly)
													{
														Blank blank = null;
														foreach (Blank current in document.Blanks)
														{
															if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
															{
																blank = current;
																break;
															}
														}
														if (blank != null && num6 > 0)
														{
															Line line = document.Lines[num6 - 1];
															int num7 = -1;
															float num8 = float.PositiveInfinity;
															for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= Math.Max(blank.StartIndex, line.StartIndex); i--)
															{
																float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
																float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
																float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
																if (num11 < num8)
																{
																	num8 = num11;
																	num7 = i;
																}
															}
															if (num7 != -1)
															{
																this.findClosestElementUp(document, num7);
															}
														}
													}
													else
													{
														if (num6 > 0)
														{
															Line line = document.Lines[num6 - 1];
															int num7 = -1;
															float num8 = float.PositiveInfinity;
															for (int i = Math.Min(this.caretIndex - 1, line.ElementCount + line.StartIndex - 1); i >= line.StartIndex; i--)
															{
																if (!document.Elements[i].InBlank)
																{
																	float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
																	float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
																	float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
																	if (num11 < num8)
																	{
																		num8 = num11;
																		num7 = i;
																	}
																}
															}
															if (num7 != -1)
															{
																this.findClosestElementUp(document, num7);
															}
														}
														else
														{
															this.findUpperLine(document);
														}
													}
												}
											}
										}
									}
									result = true;
								}
								else
								{
									if (keyData == Keys.Down)
									{
										if (this.caretContainer is Document)
										{
											Document document = this.caretContainer as Document;
											Line item = (document.Elements.Count == 0 || this.caretIndex == document.Elements.Count) ? document.LastLine : document.Elements[this.caretIndex].LineContainer;
											int num6 = document.Lines.IndexOf(item);
											if (this.ReadOnly)
											{
												Blank blank = null;
												foreach (Blank current in document.Blanks)
												{
													if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
													{
														blank = current;
														break;
													}
												}
												if (blank != null && num6 < document.Lines.Count - 1)
												{
													Line line2 = document.Lines[num6 + 1];
													int num7 = -1;
													float num8 = float.PositiveInfinity;
													for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < Math.Min(blank.StartIndex + blank.Count, line2.StartIndex + line2.ElementCount); i++)
													{
														float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
														float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
														float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
														if (num11 < num8)
														{
															num8 = num11;
															num7 = i;
														}
													}
													if (num7 != -1)
													{
														this.findClosestElementDown(document, num7);
													}
												}
											}
											else
											{
												if (num6 < document.Lines.Count - 1)
												{
													Line line2 = document.Lines[num6 + 1];
													int num7 = -1;
													float num8 = float.PositiveInfinity;
													for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.ElementCount + line2.StartIndex; i++)
													{
														if (!document.Elements[i].InBlank)
														{
															float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
															float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
															float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
															if (num11 < num8)
															{
																num8 = num11;
																num7 = i;
															}
														}
													}
													if (num7 != -1)
													{
														this.findClosestElementDown(document, num7);
													}
												}
												else
												{
													this.findDownLine(document);
												}
											}
										}
										else
										{
											if (this.caretContainer is lineexpression)
											{
												lineexpression lineexpression = this.caretContainer as lineexpression;
												if (lineexpression.DownLineExpression != null)
												{
													this.findClosestElementDown(lineexpression.DownLineExpression);
												}
												else
												{
													while (!(lineexpression is containerexpression) && lineexpression != null)
													{
														lineexpression = lineexpression.ParentExpression.ParentExpression;
													}
													if (lineexpression != null)
													{
														ExpressionInfo info = (lineexpression as containerexpression).Info;
														Document document = info.DocumentContainer;
														Line item = info.LineContainer;
														int num6 = document.Lines.IndexOf(item);
														if (this.ReadOnly)
														{
															Blank blank = null;
															foreach (Blank current in document.Blanks)
															{
																if (current.StartIndex <= this.caretIndex && current.StartIndex + current.Count >= this.caretIndex)
																{
																	blank = current;
																	break;
																}
															}
															if (blank != null && num6 < document.Lines.Count - 1)
															{
																Line line2 = document.Lines[num6 + 1];
																int num7 = -1;
																float num8 = float.PositiveInfinity;
																for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < Math.Min(blank.StartIndex + blank.Count, line2.StartIndex + line2.ElementCount); i++)
																{
																	float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
																	float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
																	float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
																	if (num11 < num8)
																	{
																		num8 = num11;
																		num7 = i;
																	}
																}
																if (num7 != -1)
																{
																	this.findClosestElementDown(document, num7);
																}
															}
														}
														else
														{
															if (num6 < document.Lines.Count - 1)
															{
																Line line2 = document.Lines[num6 + 1];
																int num7 = -1;
																float num8 = float.PositiveInfinity;
																for (int i = Math.Max(this.caretIndex + 1, line2.StartIndex); i < line2.ElementCount + line2.StartIndex; i++)
																{
																	if (!document.Elements[i].InBlank)
																	{
																		float num9 = this.virtualCaretX - document.Elements[i].OutLocation.X - document.Elements[i].OutSize.Width;
																		float num10 = this.virtualCaretX - document.Elements[i].OutLocation.X;
																		float num11 = (num9 * num10 > 0f) ? Math.Abs(num9 + num10) : 0f;
																		if (num11 < num8)
																		{
																			num8 = num11;
																			num7 = i;
																		}
																	}
																}
																if (num7 != -1)
																{
																	this.findClosestElementDown(document, num7);
																}
															}
															else
															{
																this.findDownLine(document);
															}
														}
													}
												}
											}
										}
										result = true;
									}
									else
									{
										result = base.ProcessCmdKey(ref msg, keyData);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private Cell PointInTable(Point pointLocation, TableInfo table, bool ignoreMouseState = false)
		{
			Cell result;
			if (!ignoreMouseState)
			{
				if ((float)pointLocation.Y < table.Location.Y + (float)(table.Margin.Top * 2) + table.LineWidth && (float)pointLocation.Y >= table.Location.Y && (float)pointLocation.X < table.Location.X + (float)(table.Margin.Left * 2) + table.LineWidth && (float)pointLocation.X >= table.Location.X)
				{
					this.mouseState = SuperBox.MouseState.AboveImage;
					this.mouseContainer = table;
					result = null;
					return result;
				}
			}
			for (int i = 0; i < table.TableSize.X; i++)
			{
				for (int j = 0; j < table.TableSize.Y; j++)
				{
					Cell cell = table.Rows[i].Cells[j];
					if (!cell.ismerged)
					{
						if ((float)pointLocation.Y < cell.DocLocation.Y + (float)cell.Margin.Top && (float)pointLocation.Y >= cell.DocLocation.Y - (float)cell.Margin.Bottom - table.LineWidth)
						{
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.DragBoundNS;
								this.boundDragedObject = cell;
								this.dragBound = SuperBox.Bound.Top;
							}
							result = null;
							return result;
						}
						if ((float)pointLocation.Y < cell.DocLocation.Y + table.Rows[i].Height + (float)cell.Margin.Top + table.LineWidth && (float)pointLocation.Y >= cell.DocLocation.Y + table.Rows[i].Height - (float)cell.Margin.Bottom)
						{
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.DragBoundNS;
								this.boundDragedObject = cell;
								this.dragBound = SuperBox.Bound.Bottom;
							}
							result = null;
							return result;
						}
						if ((float)pointLocation.X < cell.DocLocation.X + (float)cell.Margin.Left && (float)pointLocation.X >= cell.DocLocation.X - (float)cell.Margin.Right - table.LineWidth)
						{
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.DragBoundWE;
								this.boundDragedObject = cell;
								this.dragBound = SuperBox.Bound.Left;
							}
							result = null;
							return result;
						}
						if ((float)pointLocation.X < cell.DocLocation.X + table.Columns[j].Width + (float)cell.Margin.Left + table.LineWidth && (float)pointLocation.X >= cell.DocLocation.X + table.Columns[j].Width - (float)cell.Margin.Right)
						{
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.DragBoundWE;
								this.boundDragedObject = cell;
								this.dragBound = SuperBox.Bound.Right;
							}
							result = null;
							return result;
						}
						if ((float)pointLocation.Y < cell.DocLocation.Y + table.Rows[i].Height && (float)pointLocation.Y >= cell.DocLocation.Y && (float)pointLocation.X < cell.DocLocation.X + table.Columns[j].Width && (float)pointLocation.X >= cell.DocLocation.X)
						{
							this.boundDragedObject = null;
							result = cell;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}
		private bool PointInBlank(Document document, Point pointLocation, bool ignoreMouseState)
		{
			bool result;
			foreach (Blank current in document.Blanks)
			{
				if (current.Region.IsVisible(pointLocation))
				{
					if (!ignoreMouseState)
					{
						this.mouseState = SuperBox.MouseState.InBlank;
					}
					result = true;
					return result;
				}
			}
			foreach (Element current2 in document.Elements)
			{
				if (current2 is TableInfo)
				{
					foreach (Cell current3 in (current2 as TableInfo).Items)
					{
						if (this.PointInBlank(current3, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
				if (current2 is PictureInfo)
				{
					foreach (Document current4 in (current2 as PictureInfo).Documents)
					{
						if (this.PointInBlank(current4, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}
		private bool PointInOptions(Document document, Point pointLocation, bool ignoreMouseState)
		{
			bool result;
			foreach (Options current in document.Optionss)
			{
				if (current.Region.IsVisible(pointLocation))
				{
					if (!ignoreMouseState)
					{
						this.mouseState = SuperBox.MouseState.Choice;
					}
					result = true;
					return result;
				}
			}
			foreach (Element current2 in document.Elements)
			{
				if (current2 is TableInfo)
				{
					foreach (Cell current3 in (current2 as TableInfo).Items)
					{
						if (this.PointInOptions(current3, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
				if (current2 is PictureInfo)
				{
					foreach (Document current4 in (current2 as PictureInfo).Documents)
					{
						if (this.PointInOptions(current4, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}
		private bool PointInOperation(Document document, Point pointLocation, bool ignoreMouseState)
		{
			bool result;
			foreach (Element current in document.Elements)
			{
				if (current is OperationInfo)
				{
					OperationInfo operationInfo = current as OperationInfo;
					if (operationInfo.Region.IsVisible(pointLocation))
					{
						if (!ignoreMouseState)
						{
							this.mouseState = SuperBox.MouseState.InOperate;
						}
						result = true;
						return result;
					}
				}
			}
			foreach (Element current in document.Elements)
			{
				if (current is TableInfo)
				{
					foreach (Cell current2 in (current as TableInfo).Items)
					{
						if (this.PointInOperation(current2, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
				if (current is PictureInfo)
				{
					foreach (Document current3 in (current as PictureInfo).Documents)
					{
						if (this.PointInOperation(current3, pointLocation, ignoreMouseState))
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}
		private void FindMouse(Document document, Point mousePosition, bool ignoreMouseState = false)
		{
			int num = 0;
			while (!this.freshed && num < 5)
			{
				Thread.Sleep(50);
			}
			if (num < 5)
			{
				bool flag = false;
				if (this.ReadOnly)
				{
					if (this.PointInBlank(document, mousePosition, ignoreMouseState))
					{
						flag = true;
					}
					else
					{
						if (this.PointInOptions(document, mousePosition, ignoreMouseState) || this.PointInOperation(document, mousePosition, ignoreMouseState))
						{
							return;
						}
					}
				}
				else
				{
					if (this.PointInBlank(document, mousePosition, ignoreMouseState) || this.PointInOptions(document, mousePosition, ignoreMouseState) || this.PointInOperation(document, mousePosition, ignoreMouseState))
					{
						return;
					}
				}
				if (!this.ReadOnly || flag)
				{
					foreach (Element current in document.Elements)
					{
						if (current is ExpressionInfo && (current as ExpressionInfo).Region.IsVisible(mousePosition))
						{
							ExpressionInfo expressionInfo = current as ExpressionInfo;
							expression expression = expressionInfo.ContainerExpression.PointInExpression(mousePosition);
							if (expression == null)
							{
								if (Math.Abs((float)mousePosition.X - expressionInfo.OutLocation.X) < Math.Abs((float)mousePosition.X - expressionInfo.OutLocation.X - expressionInfo.OutSize.Width))
								{
									this.mouseIndex = (float)expressionInfo.Index;
								}
								else
								{
									this.mouseIndex = (float)(expressionInfo.Index + 1);
								}
								this.mouseContainer = expressionInfo.DocumentContainer;
							}
							else
							{
								if (expression is lineexpression)
								{
									lineexpression lineexpression = expression as lineexpression;
									this.mouseIndex = 0f;
									for (int i = 0; i < lineexpression.Child.Count; i++)
									{
										if ((float)mousePosition.X >= lineexpression.Child[i].InputLocation.X && (float)mousePosition.X <= lineexpression.Child[i].InputLocation.X + lineexpression.Child[i].Region.Width)
										{
											this.mouseIndex = (float)i + ((float)mousePosition.X - lineexpression.Child[i].InputLocation.X) / lineexpression.Child[i].Region.Width;
											break;
										}
									}
									this.mouseContainer = lineexpression;
								}
								else
								{
									structexpression structexpression = expression as structexpression;
									this.mouseIndex = (float)structexpression.ParentExpression.Child.IndexOf(structexpression) + Math.Abs((float)mousePosition.X - structexpression.InputLocation.X) / structexpression.Region.Width;
									this.mouseContainer = structexpression.ParentExpression;
								}
							}
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.Select;
							}
							return;
						}
						if (current is TableInfo && (current as TableInfo).Region.IsVisible(mousePosition))
						{
							Cell cell = this.PointInTable(mousePosition, current as TableInfo, ignoreMouseState);
							if (cell != null)
							{
								this.FindMouse(cell, mousePosition, ignoreMouseState);
							}
							return;
						}
						if (current is PictureInfo && (current as PictureInfo).Region.IsVisible(mousePosition))
						{
							PictureInfo pictureInfo = current as PictureInfo;
							if (!pictureInfo.AloneSelected)
							{
								foreach (Document current2 in pictureInfo.Documents)
								{
									Region region = new Region(new RectangleF(current2.DocLocation, new SizeF(current2.OutWidth, current2.OutHeight)));
									if (region.IsVisible(mousePosition))
									{
										this.FindMouse(current2, mousePosition, ignoreMouseState);
										return;
									}
								}
								if (!ignoreMouseState)
								{
									this.mouseState = SuperBox.MouseState.AboveImage;
									this.mouseContainer = pictureInfo;
								}
								return;
							}
							this.mouseContainer = pictureInfo;
							if ((float)mousePosition.Y < pictureInfo.Location.Y + (float)(pictureInfo.Margin.Top * 2) && (float)mousePosition.Y >= pictureInfo.Location.Y)
							{
								if (!ignoreMouseState)
								{
									this.mouseState = SuperBox.MouseState.DragBoundNS;
									this.boundDragedObject = pictureInfo;
									this.dragBound = SuperBox.Bound.Top;
								}
								return;
							}
							if ((float)mousePosition.Y < pictureInfo.Location.Y + pictureInfo.Size.Height && (float)mousePosition.Y >= pictureInfo.Location.Y + pictureInfo.Size.Height - (float)(pictureInfo.Margin.Bottom * 2))
							{
								if (!ignoreMouseState)
								{
									this.mouseState = SuperBox.MouseState.DragBoundNS;
									this.boundDragedObject = pictureInfo;
									this.dragBound = SuperBox.Bound.Bottom;
								}
								return;
							}
							if ((float)mousePosition.X < pictureInfo.Location.X + (float)(pictureInfo.Margin.Left * 2) && (float)mousePosition.X >= pictureInfo.Location.X)
							{
								if (!ignoreMouseState)
								{
									this.mouseState = SuperBox.MouseState.DragBoundWE;
									this.boundDragedObject = pictureInfo;
									this.dragBound = SuperBox.Bound.Left;
								}
								return;
							}
							if ((float)mousePosition.X < pictureInfo.Location.X + pictureInfo.Size.Width && (float)mousePosition.X >= pictureInfo.Location.X + pictureInfo.Size.Width - (float)(pictureInfo.Margin.Right * 2))
							{
								if (!ignoreMouseState)
								{
									this.mouseState = SuperBox.MouseState.DragBoundWE;
									this.boundDragedObject = pictureInfo;
									this.dragBound = SuperBox.Bound.Right;
								}
								return;
							}
							if (!ignoreMouseState)
							{
								this.mouseState = SuperBox.MouseState.AboveImage;
							}
							return;
						}
					}
					this.mouseContainer = document;
					if (!ignoreMouseState)
					{
						this.mouseState = SuperBox.MouseState.Select;
					}
					if (document.Elements.Count == 0)
					{
						this.mouseIndex = 0f;
					}
					else
					{
						if (document.Lines.Count == 0 || (float)mousePosition.Y < document.DocLocation.Y + (float)document.Margin.Top)
						{
							this.mouseIndex = 0f;
						}
						else
						{
							if ((float)mousePosition.Y > document.Lines[document.Lines.Count - 1].Top + document.Lines[document.Lines.Count - 1].Height || (float)mousePosition.Y > document.DocLocation.Y + document.OutHeight - (float)document.Margin.Bottom)
							{
								this.mouseIndex = (float)document.Elements.Count;
							}
							else
							{
								foreach (Line current3 in document.Lines)
								{
									int num2 = document.Lines.IndexOf(current3);
									Region region;
									if (num2 < document.Lines.Count - 1)
									{
										Line line = document.Lines[num2 + 1];
										region = new Region(new RectangleF(document.DocLocation.X, current3.Top, document.OutWidth, line.Top - current3.Top));
									}
									else
									{
										region = new Region(new RectangleF(document.DocLocation.X, current3.Top, document.OutWidth, document.DocLocation.Y + document.OutHeight - current3.Top));
									}
									if (region.IsVisible(mousePosition))
									{
										for (int j = current3.StartIndex; j < current3.StartIndex + current3.ElementCount; j++)
										{
											if (document.Elements[j].Region.IsVisible(mousePosition))
											{
												this.mouseIndex = (float)j + ((float)mousePosition.X - document.Elements[j].Location.X) / document.Elements[j].OutSize.Width;
												return;
											}
										}
										if (current3.ElementCount > 0 && (float)mousePosition.X <= document.Elements[current3.StartIndex].OutLocation.X)
										{
											this.mouseIndex = (float)current3.StartIndex;
											break;
										}
										if (current3.ElementCount == 0)
										{
											this.mouseIndex = (float)current3.StartIndex;
											break;
										}
										for (int j = current3.StartIndex; j < current3.StartIndex + current3.ElementCount; j++)
										{
											if ((float)mousePosition.X < document.Elements[j].OutLocation.X)
											{
												this.mouseIndex = (float)j;
												return;
											}
										}
										this.mouseIndex = (float)(current3.StartIndex + current3.ElementCount);
									}
								}
							}
						}
					}
				}
				else
				{
					this.mouseState = SuperBox.MouseState.None;
				}
			}
		}
		private void SuperBox_MouseDown(object sender, MouseEventArgs e)
		{
			this.FindMouse(this.mainDocument, e.Location, false);
			if (this.mouseState == SuperBox.MouseState.AboveImage)
			{
				this.CancelSelection();
				if (this.mouseContainer is Pic_Tab)
				{
					Pic_Tab pic_Tab = this.mouseContainer as Pic_Tab;
					this.caretContainer = pic_Tab.DocumentContainer;
					this.caretIndex = pic_Tab.Index;
					pic_Tab.AloneSelected = true;
					this.selectedObject = pic_Tab;
				}
				this.hasCaret = false;
				this.UpdateCaretLocation();
				this.virtualCaretX = this.caretLocation.X;
				base.Invalidate();
			}
			else
			{
				if (this.mouseState == SuperBox.MouseState.DragBoundNS || this.mouseState == SuperBox.MouseState.DragBoundWE)
				{
					this.hasCaret = false;
					this.UpdateCaretLocation();
					this.virtualCaretX = this.caretLocation.X;
				}
				else
				{
					if (this.mouseState == SuperBox.MouseState.DragElement)
					{
						if (this.selectedObject != null)
						{
							this.selectedObject.AloneSelected = false;
							this.selectedObject = null;
						}
						this.hasCaret = false;
						this.UpdateCaretLocation();
						this.virtualCaretX = this.caretLocation.X;
					}
					else
					{
						if (this.mouseState == SuperBox.MouseState.None)
						{
							if (this.selectedObject != null)
							{
								this.selectedObject.AloneSelected = false;
								this.selectedObject = null;
							}
							this.hasCaret = false;
							this.UpdateCaretLocation();
							this.virtualCaretX = this.caretLocation.X;
						}
						else
						{
							if (this.mouseState == SuperBox.MouseState.InBlank)
							{
								if (this.selectedObject != null)
								{
									this.selectedObject.AloneSelected = false;
									this.selectedObject = null;
								}
								this.hasCaret = false;
								this.UpdateCaretLocation();
								this.virtualCaretX = this.caretLocation.X;
							}
							else
							{
								if (this.mouseState == SuperBox.MouseState.Choice)
								{
									if (this.selectedObject != null)
									{
										this.selectedObject.AloneSelected = false;
										this.selectedObject = null;
									}
									this.hasCaret = false;
									this.mainDocument.Checked(e.Location);
									if (this.ContentChanged != null)
									{
										this.ContentChanged(this, new EventArgs());
									}
									this.UpdateCaretLocation();
									this.virtualCaretX = this.caretLocation.X;
									base.Invalidate();
								}
								else
								{
									if (this.mouseState == SuperBox.MouseState.InOperate)
									{
										if (this.selectedObject != null)
										{
											this.selectedObject.AloneSelected = false;
											this.selectedObject = null;
										}
										this.hasCaret = false;
										this.mainDocument.Operate(e.Location);
										this.UpdateCaretLocation();
										this.virtualCaretX = this.caretLocation.X;
										base.Invalidate();
									}
									else
									{
										bool flag = false;
										for (int i = 0; i < this.selectedContainers.Count; i++)
										{
											object obj = this.selectedContainers[i];
											int num = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
											int num2 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
											Region region = new Region(new RectangleF(0f, 0f, 0f, 0f));
											if (this.selectedContainers[i] is Document)
											{
												Document document = this.selectedContainers[i] as Document;
												float num3;
												int num4;
												if (document.Elements.Count > 0 && num != 0)
												{
													Element element = document.Elements[num - 1];
													num3 = element.Location.X + element.OutSize.Width;
													num4 = document.Lines.IndexOf(element.LineContainer);
												}
												else
												{
													num3 = document.DocLocation.X + (float)document.Margin.Left;
													num4 = 0;
												}
												float num5;
												int num6;
												if (document.Elements.Count > 0 && num2 != 0)
												{
													Element element2 = document.Elements[num2 - 1];
													num5 = element2.Location.X + element2.OutSize.Width;
													num6 = document.Lines.IndexOf(element2.LineContainer);
												}
												else
												{
													num5 = document.DocLocation.X + (float)document.Margin.Left;
													num6 = 0;
												}
												if (num4 == num6)
												{
													region.Union(new RectangleF(num3, document.Lines[num4].Top, num5 - num3, document.Lines[num4].Height));
												}
												else
												{
													region.Union(new RectangleF(num3, document.Lines[num4].Top, document.Lines[num4].Right - num3, document.Lines[num4].Height));
													for (int j = num4 + 1; j < num6; j++)
													{
														region.Union(new RectangleF(document.Lines[j].Left, document.Lines[j].Top, document.Lines[j].Right - document.Lines[j].Left, document.Lines[j].Height));
													}
													region.Union(new RectangleF(document.Lines[num6].Left, document.Lines[num6].Top, num5 - document.Lines[num6].Left, document.Lines[num6].Height));
												}
											}
											else
											{
												if (this.selectedContainers[i] is lineexpression)
												{
													lineexpression lineexpression = this.selectedContainers[i] as lineexpression;
													for (int k = num; k < Math.Min(num2, lineexpression.Child.Count); k++)
													{
														if (lineexpression.Child != null && lineexpression.Child.Count > k)
														{
															region.Union(new RectangleF(lineexpression.Child[k].InputLocation, lineexpression.Child[k].Region));
														}
													}
												}
												else
												{
													if (this.selectedContainers[i] is TableInfo)
													{
														TableInfo tableInfo = this.selectedContainers[i] as TableInfo;
														for (int k = num; k < Math.Min(num2, tableInfo.Items.Count); k++)
														{
															Cell cell = tableInfo.Items[k];
															region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
														}
													}
													else
													{
														if (this.selectedContainers[i] is PictureInfo)
														{
															PictureInfo pictureInfo = this.selectedContainers[i] as PictureInfo;
															for (int k = num; k < Math.Min(num2, pictureInfo.Documents.Count); k++)
															{
																Document document2 = pictureInfo.Documents[k];
																region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
															}
														}
													}
												}
											}
											if (region.IsVisible(e.Location))
											{
												flag = true;
												break;
											}
										}
										if (flag)
										{
											if (this.selectedObject != null)
											{
												this.selectedObject.AloneSelected = false;
												this.selectedObject = null;
											}
											this.hasCaret = false;
											this.UpdateCaretLocation();
											this.virtualCaretX = this.caretLocation.X;
											this.mouseState = SuperBox.MouseState.DragElement;
											this.ToolStripState = SuperBox.ToolStripMenuType.General;
										}
										else
										{
											this.ToolStripState = (SuperBox.ToolStripMenuType.General | SuperBox.ToolStripMenuType.Insert);
											if ((Control.ModifierKeys & Keys.Control) == Keys.Control && this.mouseState == SuperBox.MouseState.Select)
											{
												if (this.selectedObject != null)
												{
													this.selectedObject.AloneSelected = false;
													this.selectedObject = null;
												}
											}
											else
											{
												this.CancelSelection();
											}
											if (this.mouseContainer is Document && (int)((double)this.mouseIndex + 0.5) > 0)
											{
												Element element3 = ((Document)this.mouseContainer).Elements[(int)((double)this.mouseIndex + 0.5) - 1];
												if (element3 is CharInfo && (element3 as CharInfo).Char == '\r')
												{
													Region region2 = new Region(new RectangleF(((Document)this.mouseContainer).DocLocation.X, element3.LineContainer.Top, ((Document)this.mouseContainer).DocWidth, element3.LineContainer.Height));
													if (region2.IsVisible(e.Location))
													{
														this.mouseIndex -= 1f;
													}
												}
											}
											this.selectedTextLast.Add((int)((double)this.mouseIndex + 0.5));
											this.selectedTextNew.Add((int)((double)this.mouseIndex + 0.5));
											this.selectedContainers.Add(this.mouseContainer);
											this.caretContainer = this.mouseContainer;
											this.caretIndex = (int)((double)this.mouseIndex + 0.5);
											this.beginSelectContainer = this.mouseContainer;
											this.beginSelectIndex = this.mouseIndex;
											this.hasCaret = true;
											this.UpdateCaretLocation();
											this.virtualCaretX = this.caretLocation.X;
											base.Invalidate();
										}
									}
								}
							}
						}
					}
				}
			}
		}
		private void SuperBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				this.FindMouse(this.mainDocument, e.Location, false);
				bool flag = false;
				for (int i = 0; i < this.selectedContainers.Count; i++)
				{
					int num = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
					int num2 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
					Region region = new Region(new RectangleF(0f, 0f, 0f, 0f));
					if (this.selectedContainers[i] is Document)
					{
						Document document = this.selectedContainers[i] as Document;
						float num3;
						int num4;
						if (document.Elements.Count > 0 && num != 0)
						{
							Element element = document.Elements[num - 1];
							num3 = element.Location.X + element.OutSize.Width;
							num4 = document.Lines.IndexOf(element.LineContainer);
						}
						else
						{
							num3 = document.DocLocation.X + (float)document.Margin.Left;
							num4 = 0;
						}
						float num5;
						int num6;
						if (document.Elements.Count > 0 && num2 != 0)
						{
							Element element2 = document.Elements[num2 - 1];
							num5 = element2.Location.X + element2.OutSize.Width;
							num6 = document.Lines.IndexOf(element2.LineContainer);
						}
						else
						{
							num5 = document.DocLocation.X + (float)document.Margin.Left;
							num6 = 0;
						}
						if (num4 == num6)
						{
							region.Union(new RectangleF(num3, document.Lines[num4].Top, num5 - num3, document.Lines[num4].Height));
						}
						else
						{
							region.Union(new RectangleF(num3, document.Lines[num4].Top, document.Lines[num4].Right - num3, document.Lines[num4].Height));
							for (int j = num4 + 1; j < num6; j++)
							{
								region.Union(new RectangleF(document.Lines[j].Left, document.Lines[j].Top, document.Lines[j].Right - document.Lines[j].Left, document.Lines[j].Height));
							}
							region.Union(new RectangleF(document.Lines[num6].Left, document.Lines[num6].Top, num5 - document.Lines[num6].Left, document.Lines[num6].Height));
						}
					}
					else
					{
						if (this.selectedContainers[i] is lineexpression)
						{
							lineexpression lineexpression = this.selectedContainers[i] as lineexpression;
							for (int k = num; k < Math.Min(num2, lineexpression.Child.Count); k++)
							{
								if (lineexpression.Child != null && lineexpression.Child.Count > k)
								{
									region.Union(new RectangleF(lineexpression.Child[k].InputLocation, lineexpression.Child[k].Region));
								}
							}
						}
						else
						{
							if (this.selectedContainers[i] is TableInfo)
							{
								TableInfo tableInfo = this.selectedContainers[i] as TableInfo;
								for (int k = num; k < Math.Min(num2, tableInfo.Items.Count); k++)
								{
									Cell cell = tableInfo.Items[k];
									region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
								}
							}
							else
							{
								if (this.selectedContainers[i] is PictureInfo)
								{
									PictureInfo pictureInfo = this.selectedContainers[i] as PictureInfo;
									for (int k = num; k < Math.Min(num2, pictureInfo.Documents.Count); k++)
									{
										Document document2 = pictureInfo.Documents[k];
										region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
									}
								}
							}
						}
					}
					if (region.IsVisible(e.Location))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.Cursor = Cursors.Arrow;
				}
				else
				{
					switch (this.mouseState)
					{
					case SuperBox.MouseState.None:
						this.Cursor = Cursors.Arrow;
						break;
					case SuperBox.MouseState.Select:
						this.Cursor = Cursors.IBeam;
						break;
					case SuperBox.MouseState.DragElement:
						this.Cursor = Cursors.Arrow;
						break;
					case SuperBox.MouseState.DragBoundNS:
						this.Cursor = Cursors.SizeNS;
						break;
					case SuperBox.MouseState.DragBoundWE:
						this.Cursor = Cursors.SizeWE;
						break;
					case SuperBox.MouseState.AboveImage:
						this.Cursor = Cursors.SizeAll;
						break;
					case SuperBox.MouseState.Choice:
						this.Cursor = Cursors.Hand;
						break;
					case SuperBox.MouseState.InBlank:
						this.Cursor = Cursors.Hand;
						break;
					case SuperBox.MouseState.InOperate:
						this.Cursor = Cursors.Hand;
						break;
					}
				}
			}
			else
			{
				if (this.mouseState == SuperBox.MouseState.Select)
				{
					this.FindMouse(this.mainDocument, e.Location, true);
					int l = 0;
					while (l < this.selectedContainers.Count - 2)
					{
						object obj = this.selectedContainers[l];
						int num = Math.Min(this.selectedTextLast[l], this.selectedTextNew[l]);
						int num2 = Math.Max(this.selectedTextLast[l], this.selectedTextNew[l]);
						Region region = new Region(new RectangleF(0f, 0f, 0f, 0f));
						if (this.selectedContainers[l] is Document)
						{
							Document document = this.selectedContainers[l] as Document;
							float num3;
							int num4;
							if (document.Elements.Count > 0 && num != 0)
							{
								Element element = document.Elements[num - 1];
								num3 = element.Location.X + element.OutSize.Width;
								num4 = document.Lines.IndexOf(element.LineContainer);
							}
							else
							{
								num3 = document.DocLocation.X + (float)document.Margin.Left;
								num4 = 0;
							}
							float num5;
							int num6;
							if (document.Elements.Count > 0 && num2 != 0)
							{
								Element element2 = document.Elements[num2 - 1];
								num5 = element2.Location.X + element2.OutSize.Width;
								num6 = document.Lines.IndexOf(element2.LineContainer);
							}
							else
							{
								num5 = document.DocLocation.X + (float)document.Margin.Left;
								num6 = 0;
							}
							if (num4 == num6)
							{
								region.Union(new RectangleF(num3, document.Lines[num4].Top, num5 - num3, document.Lines[num4].Height));
							}
							else
							{
								region.Union(new RectangleF(num3, document.Lines[num4].Top, document.Lines[num4].Right - num3, document.Lines[num4].Height));
								for (int j = num4 + 1; j < num6; j++)
								{
									region.Union(new RectangleF(document.Lines[j].Left, document.Lines[j].Top, document.Lines[j].Right - document.Lines[j].Left, document.Lines[j].Height));
								}
								region.Union(new RectangleF(document.Lines[num6].Left, document.Lines[num6].Top, num5 - document.Lines[num6].Left, document.Lines[num6].Height));
							}
						}
						else
						{
							if (this.selectedContainers[l] is lineexpression)
							{
								lineexpression lineexpression = this.selectedContainers[l] as lineexpression;
								for (int k = num; k < Math.Min(num2, lineexpression.Child.Count); k++)
								{
									if (lineexpression.Child != null && lineexpression.Child.Count > k)
									{
										region.Union(new RectangleF(lineexpression.Child[k].InputLocation, lineexpression.Child[k].Region));
									}
								}
							}
							else
							{
								if (this.selectedContainers[l] is TableInfo)
								{
									TableInfo tableInfo = this.selectedContainers[l] as TableInfo;
									for (int k = num; k < Math.Min(num2, tableInfo.Items.Count); k++)
									{
										Cell cell = tableInfo.Items[k];
										region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
									}
								}
								else
								{
									if (this.selectedContainers[l] is PictureInfo)
									{
										PictureInfo pictureInfo = this.selectedContainers[l] as PictureInfo;
										for (int k = num; k < Math.Min(num2, pictureInfo.Documents.Count); k++)
										{
											Document document2 = pictureInfo.Documents[k];
											region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
										}
									}
								}
							}
						}
						if (region.IsVisible(e.Location))
						{
							this.selectedContainers.RemoveAt(l);
							this.selectedTextLast.RemoveAt(l);
							this.selectedTextNew.RemoveAt(l);
						}
						else
						{
							l++;
						}
					}
					List<object> list = new List<object>();
					List<object> list2 = new List<object>();
					object obj2 = this.mouseContainer;
					list.Insert(0, obj2);
					while (obj2 != this.mainDocument && obj2 != null)
					{
						if (obj2 is Element)
						{
							obj2 = (obj2 as Element).DocumentContainer;
						}
						else
						{
							if (obj2 is Document)
							{
								obj2 = (obj2 as Document).Parent;
							}
							else
							{
								if (obj2 is expression)
								{
									if (obj2 is lineexpression)
									{
										if (obj2 is containerexpression)
										{
											obj2 = (obj2 as containerexpression).Info;
										}
										else
										{
											obj2 = (obj2 as lineexpression).ParentExpression;
										}
									}
									else
									{
										if (obj2 is structexpression)
										{
											obj2 = (obj2 as structexpression).ParentExpression;
										}
										else
										{
											obj2 = (obj2 as expression).ParentExpression;
										}
									}
								}
							}
						}
						list.Insert(0, obj2);
					}
					obj2 = this.beginSelectContainer;
					list2.Insert(0, obj2);
					while (obj2 != this.mainDocument && obj2 != null)
					{
						if (obj2 is Element)
						{
							obj2 = (obj2 as Element).DocumentContainer;
						}
						else
						{
							if (obj2 is Document)
							{
								obj2 = (obj2 as Document).Parent;
							}
							else
							{
								if (obj2 is expression)
								{
									if (obj2 is lineexpression)
									{
										if (obj2 is containerexpression)
										{
											obj2 = (obj2 as containerexpression).Info;
										}
										else
										{
											obj2 = (obj2 as lineexpression).ParentExpression;
										}
									}
									else
									{
										if (obj2 is structexpression)
										{
											obj2 = (obj2 as structexpression).ParentExpression;
										}
										else
										{
											obj2 = (obj2 as expression).ParentExpression;
										}
									}
								}
							}
						}
						list2.Insert(0, obj2);
					}
					if (list.Contains(null) || list2.Contains(null))
					{
						return;
					}
					object obj3 = null;
					int num7 = 0;
					for (int i = 0; i < Math.Min(list.Count, list2.Count); i++)
					{
						if (list2[i] == list[i])
						{
							obj3 = list[i];
							num7 = i;
						}
					}
					if (obj3 == null)
					{
						return;
					}
					this.selectedContainers[this.selectedContainers.Count - 1] = obj3;
					if (obj3 == this.mouseContainer && obj3 == this.beginSelectContainer)
					{
						this.selectedTextNew[this.selectedTextNew.Count - 1] = (int)(0.5 + (double)this.mouseIndex);
						this.selectedTextLast[this.selectedTextLast.Count - 1] = (int)(0.5 + (double)this.beginSelectIndex);
					}
					else
					{
						if (obj3 != this.mouseContainer && obj3 == this.beginSelectContainer)
						{
							int num8 = 0;
							int num9 = 0;
							if (obj3 is Document)
							{
								num8 = (list[num7 + 1] as Element).Index;
								num9 = (int)(0.5 + (double)this.beginSelectIndex);
								if (num8 > num9)
								{
									num8++;
								}
							}
							else
							{
								if (obj3 is TableInfo)
								{
									num8 = (obj3 as TableInfo).Items.IndexOf(list[num7 + 1] as Cell);
									num9 = (int)(0.5 + (double)this.beginSelectIndex);
									if (num8 > num9)
									{
										num8++;
									}
								}
								else
								{
									if (obj3 is PictureInfo)
									{
										num8 = (obj3 as PictureInfo).Documents.IndexOf(list[num7 + 1] as Document);
										num9 = (int)(0.5 + (double)this.beginSelectIndex);
										if (num8 > num9)
										{
											num8++;
										}
									}
									else
									{
										if (obj3 is lineexpression)
										{
											num8 = (obj3 as lineexpression).Child.IndexOf(list[num7 + 1] as structexpression);
											num9 = (int)(0.5 + (double)this.beginSelectIndex);
											if (num8 > num9)
											{
												num8++;
											}
										}
									}
								}
							}
							this.selectedTextNew[this.selectedTextNew.Count - 1] = num8;
							this.selectedTextLast[this.selectedTextLast.Count - 1] = num9;
						}
						else
						{
							if (obj3 == this.mouseContainer && obj3 != this.beginSelectContainer)
							{
								int num8 = 0;
								int num9 = 0;
								if (obj3 is Document)
								{
									num9 = (list2[num7 + 1] as Element).Index;
									num8 = (int)(0.5 + (double)this.mouseIndex);
									if (num8 < num9)
									{
										num9++;
									}
								}
								else
								{
									if (obj3 is TableInfo)
									{
										num9 = (obj3 as TableInfo).Items.IndexOf(list2[num7 + 1] as Cell);
										num8 = (int)(0.5 + (double)this.mouseIndex);
										if (num8 < num9)
										{
											num9++;
										}
									}
									else
									{
										if (obj3 is PictureInfo)
										{
											num9 = (obj3 as PictureInfo).Documents.IndexOf(list2[num7 + 1] as Document);
											num8 = (int)(0.5 + (double)this.mouseIndex);
											if (num8 < num9)
											{
												num9++;
											}
										}
										else
										{
											if (obj3 is lineexpression)
											{
												num9 = (obj3 as lineexpression).Child.IndexOf(list2[num7 + 1] as structexpression);
												num8 = (int)(0.5 + (double)this.mouseIndex);
												if (num8 < num9)
												{
													num9++;
												}
											}
										}
									}
								}
								this.selectedTextNew[this.selectedTextNew.Count - 1] = num8;
								this.selectedTextLast[this.selectedTextLast.Count - 1] = num9;
							}
							else
							{
								if (obj3 != this.mouseContainer && obj3 != this.beginSelectContainer)
								{
									int num8 = 0;
									int num9 = 0;
									if (obj3 is Document)
									{
										num8 = (list[num7 + 1] as Element).Index;
										num9 = (list2[num7 + 1] as Element).Index;
										if (num8 >= num9)
										{
											num8++;
										}
										else
										{
											num9++;
										}
									}
									else
									{
										if (obj3 is TableInfo)
										{
											num8 = (obj3 as TableInfo).Items.IndexOf(list[num7 + 1] as Cell);
											num9 = (obj3 as TableInfo).Items.IndexOf(list2[num7 + 1] as Cell);
											if (num8 >= num9)
											{
												num8++;
											}
											else
											{
												num9++;
											}
										}
										else
										{
											if (obj3 is PictureInfo)
											{
												num8 = (obj3 as PictureInfo).Documents.IndexOf(list[num7 + 1] as Document);
												num9 = (obj3 as PictureInfo).Documents.IndexOf(list2[num7 + 1] as Document);
												if (num8 >= num9)
												{
													num8++;
												}
												else
												{
													num9++;
												}
											}
											else
											{
												if (obj3 is structexpression)
												{
													structexpression structexpression = obj3 as structexpression;
													num9 = structexpression.Child.IndexOf(list2[num7 + 1] as lineexpression);
													num8 = structexpression.Child.IndexOf(list[num7 + 1] as lineexpression);
													lineexpression parentExpression = structexpression.ParentExpression;
													this.selectedContainers[this.selectedContainers.Count - 1] = parentExpression;
													if (num9 < num8)
													{
														num9 = parentExpression.Child.IndexOf(structexpression);
														num8 = num9 + 1;
													}
													else
													{
														num8 = parentExpression.Child.IndexOf(structexpression);
														num9 = num8 + 1;
													}
												}
												else
												{
													if (obj3 is lineexpression)
													{
														num9 = (obj3 as lineexpression).Child.IndexOf(list2[num7 + 1] as structexpression);
														num8 = (obj3 as lineexpression).Child.IndexOf(list[num7 + 1] as structexpression);
														if (num8 >= num9)
														{
															num8++;
														}
														else
														{
															num9++;
														}
													}
												}
											}
										}
									}
									this.selectedTextLast[this.selectedTextLast.Count - 1] = num9;
									this.selectedTextNew[this.selectedTextNew.Count - 1] = num8;
								}
							}
						}
					}
					this.caretContainer = this.selectedContainers[this.selectedContainers.Count - 1];
					this.caretIndex = this.selectedTextNew[this.selectedTextNew.Count - 1];
					this.hasCaret = true;
					this.UpdateCaretLocation();
				}
				else
				{
					if (this.mouseState == SuperBox.MouseState.DragBoundWE)
					{
						if (this.boundDragedObject is Cell)
						{
							Cell cell2 = this.boundDragedObject as Cell;
							TableInfo tableInfo2 = cell2.Parent as TableInfo;
							Point rowColumn = tableInfo2.GetRowColumn(cell2);
							if (this.dragBound == SuperBox.Bound.Right)
							{
								for (int i = 0; i < tableInfo2.TableSize.X; i++)
								{
									Cell cell3 = tableInfo2.Rows[i].Cells[rowColumn.Y];
									cell3.DocWidth = (float)e.Location.X - cell3.DocLocation.X - (float)cell3.Margin.Horizontal;
								}
							}
							else
							{
								if (this.dragBound == SuperBox.Bound.Left)
								{
									if (rowColumn.Y > 0)
									{
										for (int i = 0; i < tableInfo2.TableSize.X; i++)
										{
											Cell cell4 = tableInfo2.Rows[i].Cells[rowColumn.Y - 1];
											cell4.DocWidth = (float)e.Location.X - cell4.DocLocation.X - (float)cell4.Margin.Horizontal;
										}
									}
									else
									{
										float num10 = tableInfo2.Location.X + (float)tableInfo2.Margin.Left + tableInfo2.LineWidth + tableInfo2.Columns[0].Width;
										tableInfo2.Location = new PointF((float)(e.Location.X - tableInfo2.Margin.Left), tableInfo2.Location.Y);
										for (int i = 0; i < tableInfo2.TableSize.X; i++)
										{
											Cell cell4 = tableInfo2.Rows[i].Cells[rowColumn.Y];
											cell4.DocWidth = num10 - (tableInfo2.Location.X + (float)tableInfo2.Margin.Left + tableInfo2.LineWidth) - (float)cell4.Margin.Horizontal;
										}
									}
								}
							}
						}
						else
						{
							if (this.boundDragedObject is PictureInfo)
							{
								PictureInfo pictureInfo2 = this.boundDragedObject as PictureInfo;
								if (this.dragBound == SuperBox.Bound.Right)
								{
									pictureInfo2.ImageShowSize = new SizeF((float)e.Location.X - pictureInfo2.Location.X - (float)pictureInfo2.Margin.Left, pictureInfo2.ImageShowSize.Height);
								}
								else
								{
									if (this.dragBound == SuperBox.Bound.Right)
									{
										float num10 = pictureInfo2.Location.X + pictureInfo2.ImageShowSize.Width + (float)pictureInfo2.Margin.Left;
										pictureInfo2.Location = new PointF((float)(e.Location.X - pictureInfo2.Margin.Left), pictureInfo2.Location.Y);
										pictureInfo2.ImageShowSize = new SizeF(num10 - (float)e.Location.X, pictureInfo2.ImageShowSize.Height);
									}
								}
							}
						}
						this.hasCaret = false;
						this.UpdateCaretLocation();
						this.virtualCaretX = this.caretLocation.X;
					}
					else
					{
						if (this.mouseState == SuperBox.MouseState.DragBoundNS)
						{
							if (this.boundDragedObject is Cell)
							{
								Cell cell2 = this.boundDragedObject as Cell;
								TableInfo tableInfo2 = cell2.Parent as TableInfo;
								Point rowColumn = tableInfo2.GetRowColumn(cell2);
								if (this.dragBound == SuperBox.Bound.Top)
								{
									if (rowColumn.X > 0)
									{
										for (int i = 0; i < tableInfo2.TableSize.Y; i++)
										{
											Cell cell3 = tableInfo2.Columns[i].Cells[rowColumn.X];
											cell3.MinHeight = (float)e.Location.Y - cell3.DocLocation.Y - (float)cell3.Margin.Vertical;
										}
									}
								}
								else
								{
									if (this.dragBound == SuperBox.Bound.Bottom)
									{
										for (int i = 0; i < tableInfo2.TableSize.Y; i++)
										{
											Cell cell3 = tableInfo2.Columns[i].Cells[rowColumn.X];
											cell3.MinHeight = (float)e.Location.Y - cell3.DocLocation.Y - (float)cell3.Margin.Vertical;
										}
									}
								}
							}
							else
							{
								if (this.boundDragedObject is PictureInfo)
								{
									PictureInfo pictureInfo2 = this.boundDragedObject as PictureInfo;
									if (this.dragBound == SuperBox.Bound.Top)
									{
										float num11 = pictureInfo2.Location.Y + (float)pictureInfo2.Margin.Top + pictureInfo2.ImageShowSize.Height;
										pictureInfo2.Location = new PointF(pictureInfo2.Location.X, (float)(e.Location.Y - pictureInfo2.Margin.Top));
										pictureInfo2.ImageShowSize = new SizeF(pictureInfo2.ImageShowSize.Width, num11 - (float)e.Location.Y);
									}
									else
									{
										if (this.dragBound == SuperBox.Bound.Bottom)
										{
											pictureInfo2.ImageShowSize = new SizeF(pictureInfo2.ImageShowSize.Width, (float)e.Location.Y - pictureInfo2.Location.Y - (float)pictureInfo2.Margin.Top);
										}
									}
								}
							}
							this.hasCaret = false;
							this.UpdateCaretLocation();
							this.virtualCaretX = this.caretLocation.X;
						}
						else
						{
							if (this.mouseState != SuperBox.MouseState.DragElement)
							{
								if (this.mouseState == SuperBox.MouseState.Choice || this.mouseState == SuperBox.MouseState.InBlank || this.mouseState == SuperBox.MouseState.InOperate)
								{
									this.Cursor = Cursors.Hand;
									this.hasCaret = false;
									this.UpdateCaretLocation();
									this.virtualCaretX = this.caretLocation.X;
								}
							}
						}
					}
				}
				base.Invalidate();
			}
		}
		private void SuperBox_MouseUp(object sender, MouseEventArgs e)
		{
			this.FindMouse(this.mainDocument, e.Location, false);
			if (this.mouseState == SuperBox.MouseState.None)
			{
				this.hasCaret = false;
			}
			else
			{
				this.hasCaret = true;
			}
			this.UpdateCaretLocation();
			this.virtualCaretX = this.caretLocation.X;
		}
		private void SuperBox_Click(object sender, EventArgs e)
		{
			base.Focus();
		}
		private void SuperBox_MouseHover(object sender, EventArgs e)
		{
			Point pointLocation = base.PointToClient(Control.MousePosition);
			if (this.PointInOptions(this.mainDocument, pointLocation, true))
			{
				this.tip.Show("点击选项答题", this, pointLocation.X + Cursor.Current.Size.Width, pointLocation.Y, 2000);
			}
		}
		private void UpdateCaretLocation()
		{
			if (this.hasCaret)
			{
				bool flag = true;
				bool flag2 = false;
				if (this.caretContainer is Document)
				{
					Document document = this.caretContainer as Document;
					if (document.Elements.Count > 0 && this.caretIndex != 0)
					{
						Element element = document.Elements[this.caretIndex - 1];
						this.Font = element.Font;
						if (this.caretHeight != (float)this.Font.Height)
						{
							this.caretHeight = (float)this.Font.Height;
							flag2 = true;
						}
						if (element is CharInfo && (element as CharInfo).Char == '\r')
						{
							Line lineContainer = element.LineContainer;
							int num = document.Lines.IndexOf(lineContainer);
							this.caretLocation.X = document.Lines[num + 1].Left;
							FontFamily fontFamily = this.Font.FontFamily;
							this.caretLocation.Y = document.Lines[num + 1].BaseLine + document.Lines[num + 1].Top - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
						}
						else
						{
							this.caretLocation.X = element.Location.X + element.OutSize.Width;
							this.caretLocation.Y = element.LineContainer.BaseLine + element.LineContainer.Top - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
						}
					}
					else
					{
						if (document.Elements.Count > 0 && this.caretIndex == 0)
						{
							Element element = document.Elements[this.caretIndex];
							this.Font = element.Font;
							if (this.caretHeight != (float)this.Font.Height)
							{
								this.caretHeight = (float)this.Font.Height;
								flag2 = true;
							}
							this.caretLocation.X = element.Location.X;
							this.caretLocation.Y = element.LineContainer.BaseLine + element.LineContainer.Top - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
						}
						else
						{
							this.Font = document.DefaultFont;
							if (this.caretHeight != (float)this.Font.Height)
							{
								this.caretHeight = (float)this.Font.Height;
								flag2 = true;
							}
							this.caretLocation.X = document.DocLocation.X + (float)document.Margin.Left;
							this.caretLocation.Y = document.Lines[0].BaseLine + document.Lines[0].Top - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
						}
					}
				}
				else
				{
					if (this.caretContainer is lineexpression)
					{
						lineexpression lineexpression = this.caretContainer as lineexpression;
						if (lineexpression.Child != null && lineexpression.Child.Count > 0)
						{
							if (this.caretIndex != 0)
							{
								structexpression structexpression = lineexpression.Child[this.caretIndex - 1];
								this.caretLocation.X = structexpression.InputLocation.X + structexpression.Region.Width;
							}
							else
							{
								structexpression structexpression = lineexpression.Child[this.caretIndex];
								this.caretLocation.X = structexpression.InputLocation.X;
							}
						}
						else
						{
							this.caretLocation.X = lineexpression.InputLocation.X;
						}
						this.Font = lineexpression.Font;
						if (this.caretHeight != (float)this.Font.Height)
						{
							this.caretHeight = (float)this.Font.Height;
							flag2 = true;
						}
						this.caretLocation.Y = lineexpression.InputLocation.Y + lineexpression.BaseLine - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
						while (!(lineexpression is containerexpression))
						{
							lineexpression = lineexpression.ParentExpression.ParentExpression;
						}
						ExpressionInfo info = (lineexpression as containerexpression).Info;
						if (info != null)
						{
							Document documentContainer = info.DocumentContainer;
							while (true)
							{
								Region region = new Region(new RectangleF(documentContainer.DocLocation.X + (float)documentContainer.Margin.Left, documentContainer.DocLocation.Y + (float)documentContainer.Margin.Top, documentContainer.DocWidth, documentContainer.DocHeight));
								if (!region.IsVisible(this.caretLocation))
								{
									break;
								}
								if (documentContainer == this.mainDocument)
								{
									goto IL_585;
								}
								documentContainer = documentContainer.Parent.DocumentContainer;
							}
							flag = false;
							IL_585:;
						}
					}
				}
				if (flag)
				{
					if (flag2)
					{
						NativeMethods.HideCaret(base.Handle);
						NativeMethods.DestroyCaret();
						NativeMethods.CreateCaret(base.Handle, IntPtr.Zero, 1, (int)this.caretHeight);
						NativeMethods.ShowCaret(base.Handle);
					}
					NativeMethods.SetCaretPos((int)this.caretLocation.X, (int)this.caretLocation.Y);
				}
				else
				{
					NativeMethods.HideCaret(base.Handle);
					NativeMethods.DestroyCaret();
					this.caretHeight = 0f;
				}
			}
			else
			{
				NativeMethods.HideCaret(base.Handle);
				NativeMethods.DestroyCaret();
				this.caretHeight = 0f;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			this.mainDocument.Draw(graphics);
			this.freshed = true;
			this.UpdateCaretLocation();
			this.virtualCaretX = this.caretLocation.X;
			if (this.caretContainer is lineexpression)
			{
				lineexpression lineexpression = this.caretContainer as lineexpression;
				while (!(lineexpression is containerexpression))
				{
					lineexpression = lineexpression.ParentExpression.ParentExpression;
				}
				ExpressionInfo info = (lineexpression as containerexpression).Info;
				if (info != null && (!this.ReadOnly || (this.ReadOnly && info.InBlank)))
				{
					Document documentContainer = info.DocumentContainer;
					graphics.Clip = new Region(new RectangleF(documentContainer.DocLocation.X + (float)documentContainer.Margin.Left, documentContainer.DocLocation.Y + (float)documentContainer.Margin.Top, documentContainer.DocWidth, documentContainer.DocHeight));
				}
				graphics.FillRectangle(Brushes.LightSteelBlue, new RectangleF((this.caretContainer as lineexpression).InputLocation, (this.caretContainer as lineexpression).Region));
				graphics.ResetClip();
				graphics.DrawRectangle(Pens.LightSkyBlue, lineexpression.InputLocation.X - (float)info.Padding.Left, lineexpression.InputLocation.Y - (float)info.Padding.Top, lineexpression.Region.Width + (float)info.Padding.Horizontal, lineexpression.Region.Height + (float)info.Padding.Vertical);
				info.Draw(graphics);
			}
			Color highLightColor = this.mainDocument.HighLightColor;
			SolidBrush solidBrush = new SolidBrush(highLightColor);
			for (int i = 0; i < this.selectedContainers.Count; i++)
			{
				int num = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
				int num2 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
				if (this.selectedContainers[i] is Document)
				{
					float num3;
					int num4;
					if ((this.selectedContainers[i] as Document).Elements.Count > 0 && num != 0)
					{
						Element element = (this.selectedContainers[i] as Document).Elements[num - 1];
						num3 = element.Location.X + element.OutSize.Width;
						num4 = (this.selectedContainers[i] as Document).Lines.IndexOf(element.LineContainer);
					}
					else
					{
						num3 = (this.selectedContainers[i] as Document).DocLocation.X + (float)(this.selectedContainers[i] as Document).Margin.Left;
						num4 = 0;
					}
					float num5;
					int num6;
					if ((this.selectedContainers[i] as Document).Elements.Count > 0 && num2 != 0)
					{
						Element element = (this.selectedContainers[i] as Document).Elements[num2 - 1];
						num5 = element.Location.X + element.OutSize.Width;
						num6 = (this.selectedContainers[i] as Document).Lines.IndexOf(element.LineContainer);
					}
					else
					{
						num5 = (this.selectedContainers[i] as Document).DocLocation.X + (float)(this.selectedContainers[i] as Document).Margin.Left;
						num6 = 0;
					}
					if (num4 == num6)
					{
						graphics.FillRectangle(solidBrush, num3, (this.selectedContainers[i] as Document).Lines[num4].Top, num5 - num3, (this.selectedContainers[i] as Document).Lines[num4].Height);
					}
					else
					{
						graphics.FillRectangle(solidBrush, num3, (this.selectedContainers[i] as Document).Lines[num4].Top, (this.selectedContainers[i] as Document).Lines[num4].Right - num3, (this.selectedContainers[i] as Document).Lines[num4].Height);
						for (int j = num4 + 1; j < num6; j++)
						{
							graphics.FillRectangle(solidBrush, (this.selectedContainers[i] as Document).Lines[j].Left, (this.selectedContainers[i] as Document).Lines[j].Top, (this.selectedContainers[i] as Document).Lines[j].Right - (this.selectedContainers[i] as Document).Lines[j].Left, (this.selectedContainers[i] as Document).Lines[j].Height);
						}
						graphics.FillRectangle(solidBrush, (this.selectedContainers[i] as Document).Lines[num6].Left, (this.selectedContainers[i] as Document).Lines[num6].Top, num5 - (this.selectedContainers[i] as Document).Lines[num6].Left, (this.selectedContainers[i] as Document).Lines[num6].Height);
					}
					(this.selectedContainers[i] as Document).DrawHighLight(graphics, num, num2 - num);
				}
				else
				{
					if (this.selectedContainers[i] is lineexpression)
					{
						for (int k = num; k < num2; k++)
						{
							if (((expression)this.selectedContainers[i]).Child != null && ((expression)this.selectedContainers[i]).Child.Count > k)
							{
								graphics.FillRectangle(solidBrush, new RectangleF(((expression)this.selectedContainers[i]).Child[k].InputLocation, ((expression)this.selectedContainers[i]).Child[k].Region));
								(this.selectedContainers[i] as lineexpression).DrawExpression(graphics);
							}
						}
					}
					else
					{
						if (this.selectedContainers[i] is TableInfo)
						{
							for (int k = num; k < num2; k++)
							{
								Cell cell = ((TableInfo)this.selectedContainers[i]).Items[k];
								graphics.FillRectangle(solidBrush, new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
								cell.Draw(graphics);
							}
						}
						else
						{
							if (this.selectedContainers[i] is PictureInfo)
							{
							}
						}
					}
				}
			}
			solidBrush.Dispose();
		}
		private void Insert()
		{
			bool flag = false;
			this.freshed = false;
			while (this.inputExpressions.Count != 0)
			{
				flag = true;
				FType fType = this.inputExpressions.Peek();
				if (this.caretContainer is Document)
				{
					Document document = this.caretContainer as Document;
					if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
					{
						int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
						int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
						this.caretIndex = num + document.DeleteElement(num, num2 - num, this.ReadOnly);
						this.selectedContainers = new List<object>();
						this.selectedTextLast = new List<int>();
						this.selectedTextNew = new List<int>();
					}
					containerexpression containerexpression = new containerexpression(Qisi.Editor.CommonMethods.GetCambriaFont(this.Font.Size, FontStyle.Regular));
					containerexpression.Child = new List<structexpression>();
					structexpression structexpression = Qisi.Editor.CommonMethods.CreateExpr(fType.ToString(), containerexpression, this.ForeColor, "", 2, 1);
					containerexpression.Child.Add(structexpression);
					if (document.InsertExpression(this.caretIndex, containerexpression, this.Font, this.ReadOnly))
					{
						if (structexpression.DefaultChild != null)
						{
							this.caretContainer = structexpression.DefaultChild;
							this.caretIndex = 0;
						}
						else
						{
							this.caretContainer = containerexpression;
							this.caretIndex = 1;
						}
					}
				}
				else
				{
					if (this.caretContainer is lineexpression)
					{
						lineexpression lineexpression = this.caretContainer as lineexpression;
						while (!(lineexpression is containerexpression))
						{
							lineexpression = lineexpression.ParentExpression.ParentExpression;
						}
						containerexpression containerexpression = lineexpression as containerexpression;
						ExpressionInfo info = containerexpression.Info;
						info.Sized = false;
						lineexpression = (this.caretContainer as lineexpression);
						if ((this.ReadOnly && info.InBlank) || !this.ReadOnly)
						{
							if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
							{
								int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
								int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
								if (lineexpression.Child != null)
								{
									lineexpression.Child.RemoveRange(num, num2 - num);
								}
								this.caretIndex = num;
								this.selectedContainers = new List<object>();
								this.selectedTextLast = new List<int>();
								this.selectedTextNew = new List<int>();
							}
							structexpression structexpression = Qisi.Editor.CommonMethods.CreateExpr(fType.ToString(), lineexpression, this.ForeColor, "", 2, 1);
							if (lineexpression.Child == null)
							{
								lineexpression.Child = new List<structexpression>();
								lineexpression.Child.Insert(0, structexpression);
							}
							else
							{
								lineexpression.Child.Insert(this.caretIndex, structexpression);
							}
							if (structexpression.DefaultChild != null)
							{
								this.caretContainer = structexpression.DefaultChild;
								this.caretIndex = 0;
							}
							else
							{
								this.caretIndex++;
							}
						}
					}
				}
				this.inputExpressions.Dequeue();
			}
			while (this.inputChars.Count != 0)
			{
				flag = true;
				char c = this.inputChars.Peek();
				if (c == '\b')
				{
					if (this.caretContainer is Document && this.caretIndex > 0)
					{
						Document document = this.caretContainer as Document;
						if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
						{
							int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
							int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
							if (num == num2)
							{
								if (document.DeleteElement(this.caretIndex - 1, this.ReadOnly))
								{
									this.caretIndex--;
								}
							}
							else
							{
								this.caretIndex = num + document.DeleteElement(num, num2 - num, this.ReadOnly);
							}
							this.selectedContainers = new List<object>();
							this.selectedTextLast = new List<int>();
							this.selectedTextNew = new List<int>();
						}
						else
						{
							if (this.selectedContainers.Count == 0)
							{
								if (document.DeleteElement(this.caretIndex - 1, this.ReadOnly))
								{
									this.caretIndex--;
								}
							}
						}
					}
					else
					{
						if (this.caretContainer is lineexpression && this.caretIndex > 0)
						{
							lineexpression lineexpression = this.caretContainer as lineexpression;
							while (!(lineexpression is containerexpression))
							{
								lineexpression = lineexpression.ParentExpression.ParentExpression;
							}
							containerexpression containerexpression = lineexpression as containerexpression;
							ExpressionInfo info = containerexpression.Info;
							info.Sized = false;
							lineexpression = (this.caretContainer as lineexpression);
							if (info.InBlank || !this.ReadOnly)
							{
								if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
								{
									int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
									int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
									if (num == num2)
									{
										if (lineexpression.Child != null)
										{
											lineexpression.Child.RemoveAt(this.caretIndex - 1);
										}
										if (this.caretIndex > 0)
										{
											this.caretIndex--;
										}
									}
									else
									{
										if (lineexpression.Child != null)
										{
											lineexpression.Child.RemoveRange(num, num2 - num);
										}
										this.caretIndex = num;
									}
									this.selectedContainers = new List<object>();
									this.selectedTextLast = new List<int>();
									this.selectedTextNew = new List<int>();
								}
								else
								{
									if (this.selectedContainers.Count == 0)
									{
										if (lineexpression.Child != null)
										{
											lineexpression.Child.RemoveAt(this.caretIndex - 1);
										}
										if (this.caretIndex > 0)
										{
											this.caretIndex--;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					if (c == '\u007f')
					{
						if (this.selectedContainers.Count == 0 || (this.selectedContainers.Count == 1 && this.selectedTextLast[0] == this.selectedTextNew[0]))
						{
							if (this.caretContainer is Document)
							{
								Document document = this.caretContainer as Document;
								if (this.caretIndex < document.Elements.Count)
								{
									document.DeleteElement(this.caretIndex, this.ReadOnly);
								}
							}
							else
							{
								if (this.caretContainer is lineexpression)
								{
									lineexpression lineexpression = this.caretContainer as lineexpression;
									while (!(lineexpression is containerexpression))
									{
										lineexpression = lineexpression.ParentExpression.ParentExpression;
									}
									ExpressionInfo info2 = (lineexpression as containerexpression).Info;
									info2.Sized = false;
									if (info2.InBlank || !this.ReadOnly)
									{
										lineexpression = (this.caretContainer as lineexpression);
										if (lineexpression.Child != null && this.caretIndex < lineexpression.Child.Count)
										{
											lineexpression.Child.RemoveAt(this.caretIndex);
										}
									}
								}
							}
						}
						else
						{
							for (int i = this.selectedContainers.Count - 1; i >= 0; i--)
							{
								int num = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
								int num2 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
								if (this.selectedContainers[i] is Document)
								{
									Document document = this.selectedContainers[i] as Document;
									int num3 = document.DeleteElement(num, num2 - num, this.ReadOnly);
									for (int j = 0; j < i; j++)
									{
										if (this.selectedContainers[j] == this.selectedContainers[i])
										{
											if (this.selectedTextLast[j] >= num2)
											{
												local List<int> list1;
												local int index1;
												(list1 = this.selectedTextLast)[index1 = j] = list1[index1] - (num2 - num - num3);
											}
											else
											{
												if (this.selectedTextLast[j] >= num)
												{
													this.selectedTextLast[j] = num;
												}
											}
											if (this.selectedTextNew[j] >= num2)
											{
												List<int> list;
												int index;
												(list = this.selectedTextNew)[index = j] = list[index] - (num2 - num - num3);
											}
											else
											{
												if (this.selectedTextNew[j] >= num)
												{
													this.selectedTextNew[j] = num;
												}
											}
										}
									}
									this.caretContainer = document;
									this.caretIndex = num + num3;
								}
								else
								{
									if (this.selectedContainers[i] is TableInfo)
									{
										TableInfo tableInfo = this.selectedContainers[i] as TableInfo;
										if (tableInfo.InBlank || !this.ReadOnly)
										{
											for (int j = num; j < num2; j++)
											{
												tableInfo.Items[j].ClearAll();
											}
											this.caretContainer = tableInfo.Items[num];
											this.caretIndex = 0;
										}
									}
									else
									{
										if (this.selectedContainers[i] is PictureInfo)
										{
											PictureInfo pictureInfo = this.selectedContainers[i] as PictureInfo;
											if (pictureInfo.InBlank || !this.ReadOnly)
											{
												for (int j = num; j < num2; j++)
												{
													((PictureInfo)this.selectedContainers[i]).Documents[j].ClearAll();
												}
												this.caretContainer = pictureInfo.Documents[0];
												this.caretIndex = 0;
											}
										}
										else
										{
											if (this.selectedContainers[i] is lineexpression)
											{
												lineexpression lineexpression2 = this.selectedContainers[i] as lineexpression;
												while (!(lineexpression2 is containerexpression))
												{
													lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
												}
												ExpressionInfo info2 = (lineexpression2 as containerexpression).Info;
												info2.Sized = false;
												if (info2.InBlank || !this.ReadOnly)
												{
													lineexpression2 = (this.selectedContainers[i] as lineexpression);
													if (lineexpression2.Child != null)
													{
														lineexpression2.Child.RemoveRange(num, num2 - num);
													}
													for (int j = 0; j < i; j++)
													{
														if (this.selectedContainers[j] == this.selectedContainers[i])
														{
															if (this.selectedTextLast[j] >= num2)
															{
																List<int> list;
																int index;
																(list = this.selectedTextLast)[index = j] = list[index] - (num2 - num);
															}
															else
															{
																if (this.selectedTextLast[j] >= num)
																{
																	this.selectedTextLast[j] = num;
																}
															}
															if (this.selectedTextNew[j] >= num2)
															{
																List<int> list;
																int index;
																(list = this.selectedTextNew)[index = j] = list[index] - (num2 - num);
															}
															else
															{
																if (this.selectedTextNew[j] >= num)
																{
																	this.selectedTextNew[j] = num;
																}
															}
														}
													}
													this.caretContainer = this.selectedContainers[i];
													this.caretIndex = num;
												}
											}
										}
									}
								}
							}
						}
						this.selectedTextNew = new List<int>();
						this.selectedTextLast = new List<int>();
						this.selectedContainers = new List<object>();
					}
					else
					{
						if (this.caretContainer is Document)
						{
							Document document2 = this.caretContainer as Document;
							if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
							{
								int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
								int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
								this.caretIndex = num + document2.DeleteElement(num, num2 - num, this.ReadOnly);
								this.selectedContainers = new List<object>();
								this.selectedTextLast = new List<int>();
								this.selectedTextNew = new List<int>();
							}
							if (document2.InsertChar(this.caretIndex, c, this.Font, this.ReadOnly))
							{
								this.caretIndex++;
							}
						}
						else
						{
							if (this.caretContainer is lineexpression)
							{
								lineexpression lineexpression2 = this.caretContainer as lineexpression;
								while (!(lineexpression2 is containerexpression))
								{
									lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
								}
								ExpressionInfo info = ((containerexpression)lineexpression2).Info;
								info.Sized = false;
								if ((this.ReadOnly && info.InBlank) || !this.ReadOnly)
								{
									lineexpression2 = (this.caretContainer as lineexpression);
									if (this.selectedContainers.Count > 0 && this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer)
									{
										int num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
										int num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
										if (((expression)this.caretContainer).Child != null)
										{
											((expression)this.caretContainer).Child.RemoveRange(num, num2 - num);
										}
										this.caretIndex = num;
										this.selectedContainers = new List<object>();
										this.selectedTextLast = new List<int>();
										this.selectedTextNew = new List<int>();
									}
									structexpression item = Qisi.Editor.CommonMethods.CreateExpr("字符", lineexpression2, this.ForeColor, c.ToString(), 2, 1);
									if (lineexpression2.Child == null)
									{
										lineexpression2.Child = new List<structexpression>();
										lineexpression2.Child.Insert(this.caretIndex, item);
									}
									else
									{
										lineexpression2.Child.Insert(this.caretIndex, item);
									}
									while (!(lineexpression2 is containerexpression))
									{
										lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
									}
									this.caretIndex++;
								}
							}
						}
					}
				}
				this.inputChars.Dequeue();
			}
			while (this.inputImages.Count != 0)
			{
				flag = true;
				Image image = this.inputImages.Peek();
				if (this.caretContainer is Document)
				{
					Document document2 = this.caretContainer as Document;
					document2.InsertPic(this.caretIndex, image, this.Font, this.ReadOnly);
				}
				this.inputImages.Dequeue();
			}
			while (this.inputTables.Count != 0)
			{
				flag = true;
				Point tableSize = this.inputTables.Peek();
				if (this.caretContainer is Document)
				{
					Document document2 = this.caretContainer as Document;
					if (document2.InsertTable(this.caretIndex, tableSize, this.Font, this.ReadOnly))
					{
						Element element = document2.Elements[this.caretIndex];
						if (element is TableInfo)
						{
							this.caretContainer = (element as TableInfo).Items[0];
							this.caretIndex = 0;
						}
					}
				}
				this.inputTables.Dequeue();
			}
			if (flag)
			{
				this.hasCaret = true;
				base.Invalidate();
				if (this.ContentChanged != null)
				{
					this.ContentChanged(this, new EventArgs());
				}
			}
			this.freshed = true;
		}
		private bool JuZheng(expression e, PointF p)
		{
			bool flag = false;
			if (e.InputLocation.X <= p.X && p.X <= e.InputLocation.X + e.Region.Width && e.InputLocation.Y <= p.Y && p.Y <= e.InputLocation.Y + e.Region.Height)
			{
				if (e.Type == FType.矩阵)
				{
					if (e.Child == null)
					{
						flag = false;
					}
					else
					{
						foreach (expression current in e.Child)
						{
							if (current.InputLocation.X <= p.X && p.X <= current.InputLocation.X + current.Region.Width && current.InputLocation.Y <= p.Y && p.Y <= current.InputLocation.Y + current.Region.Height)
							{
								this.currentMatrixItemIndex = e.Child.IndexOf(current);
								this.currentMatrix = e;
								flag = true;
								break;
							}
						}
					}
				}
				else
				{
					if (e.Child != null)
					{
						foreach (expression current2 in e.Child)
						{
							flag |= this.JuZheng(current2, p);
						}
					}
				}
			}
			return flag;
		}
		private void 在下方插入行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void 在左侧插入列ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void 在右侧插入列ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void 在上方插入行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void 删除行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void 删除列ToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		public void AppendImage(Image img)
		{
			this.inputImages.Enqueue(img);
			this.Insert();
		}
		internal void AppendF(FType type)
		{
			this.inputExpressions.Enqueue(type);
			this.Insert();
		}
		public void AppendT(Point p)
		{
			this.inputTables.Enqueue(p);
			this.Insert();
		}
		private void mainDocument_HeightChanged(object sender, EventArgs e)
		{
			if (base.Height != (int)this.mainDocument.OutHeight)
			{
				base.Height = (int)this.mainDocument.OutHeight;
			}
		}
		private void mainDocument_ContentChanged(object sender, EventArgs e)
		{
		}
		private void mainDocument_OperateClicked(object sender, EventArgs e)
		{
			if (this.OperateClicked != null)
			{
				this.OperateClicked(sender, e);
			}
		}
		private void mainDocument_OperateDone(object sender, EventArgs e)
		{
			if (this.OperateDone != null)
			{
				if (sender is OperationInfo)
				{
					this.OperateDone(sender, new MessageEventArgs((sender as OperationInfo).OperationID));
				}
				else
				{
					this.OperateDone(sender, new MessageEventArgs(""));
				}
			}
		}
		public void DoOperate(object obj, Image stuImg, string stuInfo, string dataPath, int examLeftTime, int tipTime, string stdanswer)
		{
			if (obj != null && obj is OperationInfo)
			{
				OperationInfo operationInfo = obj as OperationInfo;
				operationInfo.Do(stuImg, stuInfo, dataPath, examLeftTime, tipTime, stdanswer);
			}
		}
		public void LoadOptionSTD(string answer)
		{
			this.mainDocument.LoadOptionSTD(answer);
			base.Invalidate();
		}
		protected override void OnLostFocus(EventArgs e)
		{
			this.hasCaret = false;
			NativeMethods.DestroyCaret();
			base.Invalidate();
			base.OnLostFocus(e);
		}
		protected override void OnGotFocus(EventArgs e)
		{
			NativeMethods.DestroyCaret();
			NativeMethods.CreateCaret(base.Handle, IntPtr.Zero, 1, (int)this.caretHeight);
			NativeMethods.ShowCaret(base.Handle);
			this.UpdateCaretLocation();
			base.OnGotFocus(e);
		}
		public void Save(string PathtoSave)
		{
			string contents = this.mainDocument.toXml(PathtoSave);
			File.WriteAllText(Path.Combine(PathtoSave, "Question.Xml"), contents, Encoding.UTF8);
		}
		public void setFont(Font F)
		{
			this.Font = F;
		}
		public string getContent()
		{
			return this.mainDocument.getContent();
		}
		public string getContentXml()
		{
			return this.mainDocument.getContentXml();
		}
		private void SuperBox_SizeChanged(object sender, EventArgs e)
		{
			if (this.mainDocument != null && this.mainDocument.DocWidth != (float)(base.Width - this.mainDocument.Margin.Horizontal))
			{
				this.mainDocument.DocWidth = (float)(base.Width - this.mainDocument.Margin.Horizontal);
			}
		}
		public void LoadFromXml(string File)
		{
			this.selectedContainers = new List<object>();
			this.selectedTextLast = new List<int>();
			this.selectedTextNew = new List<int>();
			this.mainDocument = new Document(new Padding(10, 0, 10, 10), this.Font, null, (float)base.ClientSize.Width, new PointF(0f, 0f), this.BackColor);
			this.mainDocument.ContentChanged += new EventHandler(this.mainDocument_HeightChanged);
			this.mainDocument.HeightChanged += new EventHandler(this.mainDocument_HeightChanged);
			this.mainDocument.OperateClicked += new EventHandler(this.mainDocument_OperateClicked);
			this.mainDocument.OperateDone += new EventHandler(this.mainDocument_OperateDone);
			this.mainDocument.LoadXmlFromFile(File);
			this.Draw();
		}
		public void Draw()
		{
			Graphics g = base.CreateGraphics();
			this.mainDocument.Draw(g);
		}
		public void Clear()
		{
			this.selectedContainers = new List<object>();
			this.selectedTextLast = new List<int>();
			this.selectedTextNew = new List<int>();
			this.mainDocument.ClearAll();
			base.Invalidate();
		}
		public void FillIn(string[] strs, string filepath, string id)
		{
			this.mainDocument.FillIn(strs, filepath, id);
			base.Invalidate();
		}
		private void AddLine(object sender, EventArgs e)
		{
		}
		private void Cut(object sender, EventArgs e)
		{
		}
		private void Copy(object sender, EventArgs e)
		{
		}
		private void Paste(object sender, EventArgs e)
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string fullName = typeof(Document).FullName;
			if (dataObject.GetDataPresent(fullName))
			{
				Document document = dataObject.GetData(fullName) as Document;
			}
			if (dataObject.GetDataPresent(DataFormats.Bitmap))
			{
				Bitmap img = dataObject.GetData(DataFormats.Bitmap) as Bitmap;
				this.AppendImage(img);
			}
			else
			{
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					string text = dataObject.GetData(DataFormats.Text) as string;
					char[] array = text.ToCharArray();
					for (int i = 0; i < array.Length; i++)
					{
						char item = array[i];
						this.inputChars.Enqueue(item);
					}
					this.Insert();
				}
			}
		}
		private void SelectablePaste(object sender, EventArgs e)
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string fullName = typeof(Document).FullName;
			if (dataObject.GetDataPresent(fullName))
			{
				list.Add(fullName);
				list2.Add("启思文档编辑器格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Bitmap))
			{
				list.Add(DataFormats.Bitmap);
				list2.Add("Microsoft Windows 位图数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.CommaSeparatedValue))
			{
				list.Add(DataFormats.CommaSeparatedValue);
				list2.Add("逗号分隔值 (CSV)数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Dib))
			{
				list.Add(DataFormats.Dib);
				list2.Add("与设备无关的位图 (DIB) 数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Dif))
			{
				list.Add(DataFormats.Dif);
				list2.Add("Windows 数据交换 (DIF)格式数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.EnhancedMetafile))
			{
				list.Add(DataFormats.EnhancedMetafile);
				list2.Add("Windows 增强型图元文件格式");
			}
			if (dataObject.GetDataPresent(DataFormats.FileDrop))
			{
				list.Add(DataFormats.FileDrop);
				list2.Add("Windows 文件放置格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Html))
			{
				list.Add(DataFormats.Html);
				list2.Add("HTML 数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Locale))
			{
				list.Add(DataFormats.Locale);
				list2.Add("Windows 区域设置 (区域性) 数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.MetafilePict))
			{
				list.Add(DataFormats.MetafilePict);
				list2.Add("Windows 图元文件图像数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.OemText))
			{
				list.Add(DataFormats.OemText);
				list2.Add("标准 Windows OEM 文本数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Palette))
			{
				list.Add(DataFormats.Palette);
				list2.Add("Windows 调色板数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.PenData))
			{
				list.Add(DataFormats.PenData);
				list2.Add("Windows 笔数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Riff))
			{
				list.Add(DataFormats.Riff);
				list2.Add("资源交换文件格式 (RIFF) 音频数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Rtf))
			{
				list.Add(DataFormats.Rtf);
				list2.Add("RTF 格式 (RTF) 数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Serializable))
			{
				list.Add(DataFormats.Serializable);
				list2.Add("封装任何类型可序列化数据对象的数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.StringFormat))
			{
				list.Add(DataFormats.StringFormat);
				list2.Add("公共语言运行时 (CLR) 字符串类数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.SymbolicLink))
			{
				list.Add(DataFormats.SymbolicLink);
				list2.Add("Windows 符号字符串数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				list.Add(DataFormats.Text);
				list2.Add("ANSI 文本数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.Tiff))
			{
				list.Add(DataFormats.Tiff);
				list2.Add("标记图像文件格式 (TIFF) 数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.UnicodeText))
			{
				list.Add(DataFormats.UnicodeText);
				list2.Add("Unicode 文本数据格式");
			}
			if (dataObject.GetDataPresent(DataFormats.WaveAudio))
			{
				list.Add(DataFormats.WaveAudio);
				list2.Add("波形音频数据格式");
			}
			FormFormatSelect formFormatSelect = new FormFormatSelect(list2);
			if (formFormatSelect.ShowDialog(this) == DialogResult.OK)
			{
				int selectdindex = formFormatSelect.selectdindex;
			}
		}
		private void InsertBlankEnterless(object sender, EventArgs e)
		{
			if (this.caretContainer is Document)
			{
				FormBlankInfo formBlankInfo = new FormBlankInfo();
				if (formBlankInfo.ShowDialog() == DialogResult.OK)
				{
					Document document = this.caretContainer as Document;
					document.InsertBlank(this.caretIndex, formBlankInfo.MaxCount, formBlankInfo.MinWidth, false);
				}
			}
			base.Invalidate();
		}
		private void InsertBlankEnterable(object sender, EventArgs e)
		{
			if (this.caretContainer is Document)
			{
				FormBlankInfo formBlankInfo = new FormBlankInfo();
				if (formBlankInfo.ShowDialog() == DialogResult.OK)
				{
					Document document = this.caretContainer as Document;
					document.InsertBlank(this.caretIndex, formBlankInfo.MaxCount, formBlankInfo.MinWidth, true);
				}
			}
			base.Invalidate();
		}
		private void InsertOptions(object sender, EventArgs e)
		{
			if (this.caretContainer is Document)
			{
				FormOptionInfo formOptionInfo = new FormOptionInfo();
				if (formOptionInfo.ShowDialog() == DialogResult.OK)
				{
					Document document = this.caretContainer as Document;
					document.InsertOptions(this.caretIndex, formOptionInfo.Options, formOptionInfo.Elements);
				}
			}
			base.Invalidate();
		}
	}
}

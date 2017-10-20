using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using TestScheme.Schemes;
using TestScheme.Schemes.Objects;
using TestScheme.Schemes.Objects.Elements;
using TestScheme.DrawingElements;
namespace TestScheme
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Element _newElement;
        private Element _elemSelected;

        private Point _mouseLocation;
        private Element _tmpConnectionStart;
        private Vertex _tmpConnectionEnd;

        private const double Delta = 2;


        public MainWindow()
        {
            InitializeComponent();
            Init();
            
        }

        #region Выбор элемента
        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            ElemChoosed(new Source());
        }

        private void btnPipe_Click(object sender, RoutedEventArgs e)
        {
            ElemChoosed(new Pipe());
        }

        private void btnHeatExchanger_Click(object sender, RoutedEventArgs e)
        {
            ElemChoosed(new HeatExchanger());
        }

        private void btnFlowing_Click(object sender, RoutedEventArgs e)
        {
            ElemChoosed(new Terminal());
        }

        private void ElemChoosed(Element elem)
        {
            this.Cursor = Cursors.Hand;
            _newElement = elem;
        }
        #endregion

        #region Проверка при нажатии на канву

        void CheckLocation(Point checkingPoint)
        {
            foreach (Element elem in Scheme.SchemeElements)
            {
                if (elem.CheckOutput(checkingPoint))
                {
                    _tmpConnectionStart = elem;
                    break;
                }
                else if (elem.CheckInput(checkingPoint, out _tmpConnectionEnd))
                {
                    break;
                }
                else if (elem.CheckSelection(checkingPoint) & _tmpConnectionStart == null)
                {
                    if (!Element.CheckLocationInCanvas(PaintSurface, checkingPoint)) continue;
                    _elemSelected = _elemSelected != elem ? elem : null;
                    break;
                }
            }

        }

        #endregion

        #region Перерисовка

        void ReDraw()
        {
            PaintSurface.Children.Clear();

            foreach (Element elem in Scheme.SchemeElements)
            {
                elem.Draw(PaintSurface);
                elem.DrawConnections(PaintSurface);
            }
            //foreach (Element elem in Scheme.SchemeElements)
            //    elem.DrawConnections(PaintSurface);


            _elemSelected?.DrawChoosedElem(PaintSurface);

            if (_tmpConnectionStart != null)
            {
                Ellipses elps = new Ellipses(Element.SelectedConnectingEllipseBrush, _tmpConnectionStart.OutPoint);
                elps.Draw(PaintSurface);
            }
            if (_tmpConnectionEnd != null)
            {
                Ellipses elps = new Ellipses(Element.SelectedConnectingEllipseBrush, _tmpConnectionEnd.elem.InputPoints[_tmpConnectionEnd.indexInList]);
                elps.Draw(PaintSurface);
            }
        }

    
        #endregion

        #region Ввод с клавиатуры
        private void KeyDown_Event(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (_elemSelected != null)
                {
                    _elemSelected.RemoveElem();
                    _elemSelected = null;
                    ReDraw();
                }
            }
            if (e.Key == Key.Left)
            {
                if (_elemSelected != null)
                {
                    _elemSelected.Drag(-2, 0);
                    ReDraw();
                }
            }
            if (e.Key == Key.Right)
            {
                if (_elemSelected != null)
                {
                    _elemSelected.Drag(2, 0);
                    ReDraw();
                }
            }
            if (e.Key == Key.Up)
            {
                if (_elemSelected != null)
                {
                    PaintSurface.Focus();
                    _elemSelected.Drag(0, -2);
                    ReDraw();
                }
            }
            if (e.Key == Key.Down)
            {
                if (_elemSelected != null)
                {
                    PaintSurface.Focus();
                    _elemSelected.Drag(0, 2);
                    ReDraw();
                }
            }

        }
        #endregion

        #region События мыши 

        private void PaintSurface_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseLocation = e.GetPosition(PaintSurface);
            CheckLocation(_mouseLocation);

            #region Отображение свойств
            if (_elemSelected != null)
                FillingTables();
            else
                ClearTables();
            #endregion
        }

        private void PaintSurface_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_tmpConnectionStart != null)
                switch (e.LeftButton)
                {
                    case MouseButtonState.Pressed:
                        _mouseLocation = e.GetPosition(PaintSurface);
                        Point pointStart = _tmpConnectionStart.OutPoint;
                        Point pointEnd = _mouseLocation;
                        ReDraw();
                        Lines line = new Lines(pointStart, pointEnd);
                        line.Draw(PaintSurface);

                        CheckLocation(_mouseLocation);
                        break;
                    case MouseButtonState.Released:
                        if (_tmpConnectionEnd != null)
                        {
                            int indexInScheme = _tmpConnectionEnd.elem.Id;
                            int indexInInputsList = _tmpConnectionEnd.indexInList;
                            Element.CheckElemCountInList(Scheme.SchemeElements[indexInScheme].InputElements,
                                indexInInputsList);

                            Scheme.SchemeElements[indexInScheme].InputElements[indexInInputsList] = _tmpConnectionStart;
                            Scheme.SchemeElements.Find(elem => elem == _tmpConnectionStart).OutElement =
                                _tmpConnectionEnd; 
                        }
                        _tmpConnectionStart = null;
                        _tmpConnectionEnd = null;
                        ReDraw();
                        break;
                }

            if (e.LeftButton == MouseButtonState.Pressed && _elemSelected != null)
            {
                double dx = e.GetPosition(PaintSurface).X - _mouseLocation.X;
                double dy = e.GetPosition(PaintSurface).Y - _mouseLocation.Y;

                _elemSelected.Drag(dx, dy);
                _mouseLocation = e.GetPosition(PaintSurface);
                ReDraw();
            }
        }
        private void PaintSurface_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (_tmpConnectionEnd != null)
            //{
            //    if (_tmpConnectionStart != null)
            //    {
            //        // не вызывается
            //    }
            //    _tmpConnectionStart = null;
            //    _tmpConnectionEnd = null;
            //}
            if (_newElement != null) // добавление нового элемента
            {
                _newElement.LocationPoint = e.GetPosition(PaintSurface);
                Scheme.SchemeElements.Add(_newElement);

                this.Cursor = Cursors.Arrow;
                _newElement = null;
            }
            ReDraw();

        }
        #endregion

        #region Расчет
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            Calculations.CalculateSheme(Scheme.SchemeElements);

            if (_elemSelected != null)
                FillingTables();
        }
        #endregion

        #region Tables
        private void FillingTables()
        {
            DataTable dtProperties = _elemSelected.CreateDataTableProperties();
            DataTable dtResults = _elemSelected.CreateDataTableResults();

            if (dtProperties != null)
            {
                DataGridProperties.DataContext = dtProperties.DefaultView;
                DataGridProperties.ColumnWidth = DataGridProperties.Width / 2 - Delta;
            }
            else
                DataGridProperties.DataContext = null;

            if (dtResults != null)
            {
                DataGridResults.DataContext = dtResults.DefaultView;
                DataGridResults.ColumnWidth = DataGridResults.Width / 2 - Delta;
            }
            else
                DataGridResults.DataContext = null;
        }

        private void ClearTables()
        {
            DataGridProperties.DataContext = null;
            DataGridResults.DataContext = null;
        }

        private void DataGridProperties_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //var column = e.Column as DataGridBoundColumn;
                //if (column != null)
                //{
                //    var bindingPath = (column.Binding as Binding).Path.Path;
                //    var el = e.EditingElement as TextBox;
                //    MessageBox.Show(el.Text);

                //}

                double changedValue = double.NegativeInfinity;
                int column = e.Column.DisplayIndex;
                int row = e.Row.GetIndex();

                if (e.EditingElement is TextBox changedCell)
                    double.TryParse(changedCell.Text, out changedValue);
                if (!double.IsNegativeInfinity(changedValue))
                    _elemSelected.ChangePropertyByUser(changedValue, row, column);
            }
        }


        #endregion

        #region Menu

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            Init();
            ReDraw();
        }
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            Scheme.Save();
        }
        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Scheme.Load();
            ReDraw();
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Init()
        {
            Scheme.SchemeElements = new List<Element>();

            _newElement = null;
            _elemSelected = null;
            _tmpConnectionEnd = null;
            _tmpConnectionStart = null;
            //Scheme.Count = 0;
        }
        #endregion


    }
}

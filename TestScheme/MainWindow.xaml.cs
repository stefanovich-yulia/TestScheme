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
            Scheme.SchemeElements = new List<Element>();
            Scheme.Count = 0;
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
                    if (_elemSelected == elem)
                        _elemSelected = null;
                    else
                        _elemSelected = elem;
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

            if (_elemSelected != null)
                _elemSelected.DrawChoosedElem(PaintSurface);

            if (_tmpConnectionStart != null)
            {
                _tmpConnectionStart.DrawEllipse(PaintSurface, _tmpConnectionStart.OutPoint, _tmpConnectionStart.GetSelectedConnectingEllipseBrush());
            }
            if (_tmpConnectionEnd != null)
            {
                _tmpConnectionEnd.elem.DrawEllipse(PaintSurface, _tmpConnectionEnd.elem.InputPoints[_tmpConnectionEnd.indexInList], _tmpConnectionEnd.elem.GetSelectedConnectingEllipseBrush());
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
            if ((_tmpConnectionStart != null) &  (e.LeftButton == MouseButtonState.Pressed))
            {
                Point pointStart = _tmpConnectionStart.OutPoint;
                Point pointEnd = e.GetPosition(PaintSurface);
                ReDraw();

                Lines.Draw(PaintSurface, pointStart, pointEnd);

                CheckLocation(e.GetPosition(PaintSurface));
            }
            if  ((_tmpConnectionStart!= null) & (e.LeftButton == MouseButtonState.Released))
            {
                if (_tmpConnectionEnd != null)
                {
                    int indexInScheme = Scheme.SchemeElements.IndexOf(_tmpConnectionEnd.elem);
                    int indexInInputsList = _tmpConnectionEnd.indexInList;
                    Element.CheckElemCountInList(Scheme.SchemeElements[indexInScheme].InputElements, indexInInputsList);

                    Scheme.SchemeElements[indexInScheme].InputElements[indexInInputsList] = _tmpConnectionStart;
                    Scheme.SchemeElements.Find(elem => elem == _tmpConnectionStart).OutElement = _tmpConnectionEnd; // 
                }
                _tmpConnectionStart = null;
                _tmpConnectionEnd = null;
                ReDraw();
            }

            if (_elemSelected != null & (e.LeftButton == MouseButtonState.Pressed))
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
            if (_tmpConnectionEnd != null)
            {
                if (_tmpConnectionStart != null)
                {
                    // не вызывается
                }
                _tmpConnectionStart = null;
                _tmpConnectionEnd = null;
            }
            if ( _newElement != null)
            {
                _newElement.LocationPoint = e.GetPosition(PaintSurface);
                Scheme.SchemeElements.Add(_newElement);

                this.Cursor = Cursors.Arrow;
                _newElement = null;
            }
            ReDraw();

        }
        #endregion

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            Calculations.CalculateSheme(Scheme.SchemeElements);

            if (_elemSelected != null)
                FillingTables();
        }
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

        public static DataTable DataViewAsDataTable(DataView dv)
        {
            DataTable dt = dv.Table.Clone();
            foreach (DataRowView drv in dv)
                dt.ImportRow(drv.Row);
            return dt;
        }

        private void DataGridProperties_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //DataGrid dg = (DataGrid) sender;
            //foreach (DataRowView dv in DataGridProperties.ItemsSource)
            //{
            //    MessageBox.Show(dv[1].ToString());
            //}
        }


        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            Scheme.Save();
        }

        private void DataGridProperties_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    var el = e.EditingElement as TextBox;
                    MessageBox.Show(el.Text);

                }
            }
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            Scheme.SchemeElements = new List<Element>();

            _newElement = null;
            _elemSelected = null;
            _tmpConnectionEnd = null;
            _tmpConnectionStart = null;

            ReDraw();
        }


        #endregion

        //private void DataGridProperties_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        //{
        //    if (_elemSelected != null)
        //    {
        //        //var changedText = "";
        //        //DataGrid dg = (DataGrid) sender;
        //        //DataTable dt = DataViewAsDataTable((DataView)DataGridProperties.ItemsSource);
        //        //var changedElement = e.EditingElement as TextBox;
        //        //if (changedElement!= null)
        //        //    changedText = changedElement.Text;


        //        ////double.TryParse(txt.Text, out double val);
        //        //dt.Rows[e.Row.GetIndex()].ItemArray[e.Column.DisplayIndex] = changedText;

        //        //_elemSelected.SetPropertiesFromDataTable(dt);
        //        //FillingTables();
        //    }

        //}

        //private void DataGridProperties_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    if (_elemSelected != null)
        //    {
        //        ////object s = e.EditAction;
        //        ////DataTable dt = DataViewAsDataTable((DataView)DataGridProperties.ItemsSource);


        //        ////_elemSelected.SetPropertiesFromDataTable(dt);
        //        ////FillingTables();

        //        ////DataGridProperties.CommitEdit(DataGridEditingUnit.Row, true);
        //        //DataGrid grid = (DataGrid) sender;
        //        //grid.CommitEdit(DataGridEditingUnit.Cell, true);

        //        //DataTable dt = DataViewAsDataTable((DataView)DataGridProperties.ItemsSource);


        //        //_elemSelected.SetPropertiesFromDataTable(dt);
        //        //FillingTables();
        //    }
        //}

        //private void DataGridProperties_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    //DataGridProperties.CommitEdit();

        //    //DataTable dt = DataViewAsDataTable((DataView)DataGridProperties.ItemsSource);


        //    //_elemSelected.SetPropertiesFromDataTable(dt);
        //    //FillingTables();
        //}

        //private void DataGridProperties_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    DataGrid grid = (DataGrid)sender;
        //}
    }
}

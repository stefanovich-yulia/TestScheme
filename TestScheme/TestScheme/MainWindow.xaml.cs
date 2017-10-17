using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TestScheme
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Element _newElement;
        Element _elemSelected;

        Point _mouseLocation;
        Element _tmpConnectionStart;
        Vertex _tmpConnectionEnd;


        public MainWindow()
        {
            InitializeComponent();
            Scheme.SchemeElements = new List<Element>();
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
            ElemChoosed(new Flowing());
        }

        void ElemChoosed(Element elem)
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
            paintSurface.Children.Clear();

            //var shapes = paintSurface.Children.OfType<Shape>();
            //while (shapes.Count() > 0)
            //{
            //    paintSurface.Children.Remove(shapes.First());
            //}

            foreach (Element elem in Scheme.SchemeElements)
            {
                elem.Draw(paintSurface);
                elem.DrawConnections(paintSurface);
            }

            if (_elemSelected != null)
                _elemSelected.DrawChoosedElem(paintSurface);

            //if (newElement is Lines)
            //{
            //    ConnectingOutElem.DrawEllipse(paintSurface, ConnectingOutElem.OutPoint, ConnectingOutElem.getSelectedConnectingEllipseBrush());
            //}
            //if (ConnectingInputElem != null)
            //{
            //    ConnectingInputElem.DrawEllipse(paintSurface, ConnectingInputElem.InputPoints[0], ConnectingInputElem.getSelectedConnectingEllipseBrush());
            //}

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
                    _elemSelected.Drag(-1, 0);
                    ReDraw();
                }
            }
            if (e.Key == Key.Right)
            {
                if (_elemSelected != null)
                {
                    _elemSelected.Drag(1, 0);
                    ReDraw();
                }
            }
            if (e.Key == Key.Up)
            {
                if (_elemSelected != null)
                {
                    paintSurface.Focus();
                    _elemSelected.Drag(0, -3);
                    ReDraw();
                }
            }
            if (e.Key == Key.Down)
            {
                if (_elemSelected != null)
                {
                    paintSurface.Focus();
                    _elemSelected.Drag(0, 3);
                    ReDraw();
                }
            }

        }
        #endregion

        #region События мыши 

        private void paintSurface_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseLocation = e.GetPosition(paintSurface);
            CheckLocation(_mouseLocation);
        }

        private void paintSurface_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((_tmpConnectionStart != null) & (_tmpConnectionEnd == null) &
                (e.LeftButton == MouseButtonState.Pressed))
            {
                Point pointStart = _tmpConnectionStart.OutPoint;
                Point pointEnd = e.GetPosition(paintSurface);
                ReDraw();

                Lines.Draw(paintSurface, pointStart, pointEnd);

                CheckLocation(e.GetPosition(paintSurface));
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
                double dx = e.GetPosition(paintSurface).X - _mouseLocation.X;
                double dy = e.GetPosition(paintSurface).Y - _mouseLocation.Y;

                _elemSelected.Drag(dx, dy);
                _mouseLocation = e.GetPosition(paintSurface);
                ReDraw();
            }
        }
        private void paintSurface_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                _newElement.LocationPoint = e.GetPosition(paintSurface);
                Scheme.SchemeElements.Add(_newElement);

                this.Cursor = Cursors.Arrow;
                _newElement = null;
            }
            ReDraw();

        }
        #endregion
    }
}

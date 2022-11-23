using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV;
using System.IO;

namespace MillingCutterMeasurer
{
    //Note : IMX 264   pixel size= 3.45 * 3.45 um

    public class millingCutterMeasurer
    {

        public string assembly
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string fileVersion
        {
            get
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            }
        }

        public string production
        {
            get
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
            }
        }


        //檢查邊緣圖時  大於此值為邊
        public const double EDGE_Threshold = 64;

        public double calibrationFactor;
		public double calibrationInterception;

        public double cannyThreshold = 200.0;
        public double cannyLinking = 100.0;

        private Bitmap sourceImage = null;
        public Bitmap getSourceImage()
        {
            if (this.sourceImage == null)
                return null;
            return new Bitmap(this.sourceImage);
        }

        private Image<Gray, byte> grayImage = null;
        private Image<Gray, byte> blurImage; //after pyrDown and pyrUp;

        private Image<Gray, byte> orignalEdge;
        public Bitmap getOrignalEdge()
        {
            if ( orignalEdge == null)
                return null;

            return orignalEdge.Bitmap;
        }

        private Image<Gray, byte> baseCalibBlurImage = null;
        public Bitmap getBaseCalibratedImage()
        {
            return (baseCalibBlurImage == null) ? null : this.baseCalibBlurImage.Bitmap;

        }
        private Image<Gray, byte> baseCalibEdge = null;
        public Bitmap getBaseCalibratedEdge()
        {
            return (baseCalibEdge == null) ? null : baseCalibEdge.Mat.ToImage<Bgr, byte>().Bitmap;
        }

        private Image<Gray, byte> cutterCalibBlurImage = null;
        public Bitmap getCutterCalibratedImage()
        {
            return (cutterCalibBlurImage == null) ? null : this.cutterCalibBlurImage.Bitmap;

        }
        private Image<Gray, byte> cutterCalibEdge = null;
        public Bitmap getCutterCalibratedEdge()
        {
            return (cutterCalibEdge == null) ? null : cutterCalibEdge.Mat.ToImage<Bgr, byte>().Bitmap;
        }

        private Image<Gray, byte> handleImage = null;
        public Bitmap getHandleImage()
        {
            return (this.handleImage == null) ? null : this.handleImage.Bitmap;
        }
        private Image<Gray, byte> handleEdge = null;
        public Bitmap getHandleEdge()
        {
            return (this.handleEdge == null) ? null : this.handleEdge.Bitmap;
        }
        public int[] handleEdgeHeights = null;
		public double[] handleEdgeSmoothHeights = null;
		public double[] handleEdgeSloops = null;


        //Holder Base
        //public double baseSloop = double.NaN;
        public double baseAngDiff = double.NaN;
        //public double leftIntercept = double.NaN;
        //public double rightIntercept = double.NaN;
        public double baseLeftEdgeX = double.NaN;
        public double baseRightEdgeX = double.NaN;
        //Cutter
        public int measureLeftEdgeX = -1;
        public int measureRightEdgeX = -1;
        public double cutterHandleSloop = double.NaN;
        public double cutterHandleAngDiff = double.NaN; //二次柄錐度
        public int cutterCenter = -1;  //校後刀芯Y

        public double handle_Width = double.NaN;  //柄徑
        public double handle_Width_R = double.NaN;  //左區柄徑
        public double handle_Width_L = double.NaN;  //右區柄徑
        public double handle_parallelism = double.NaN;   //柄錐度
        public double handle_Length
		{
			get
			{
				if (double.IsNaN(handle_end) || double.IsNaN(handle_start))
					return double.NaN;
				return handle_end - handle_start;
			}
		}//柄長
        public double handle_end = double.NaN;  //刀尾端
        public double handle_start = double.NaN; //柄起點

        public double full_Length = double.NaN;  //全長
        
        
        public double blade_end = double.NaN;  //刀刃端
        //blade width
        public int blade_width_check_start = -1;  //檢查起點
        public int blade_width_check_end = -1;      //檢查終得
        public double blade_inner_upper = double.NaN;
        public double blade_outer_upper = double.NaN;
        public double blade_inner_lower = double.NaN;
        public double blade_outer_lower = double.NaN;
        public double blade_inner_width
		{
			get
			{
				if (double.IsNaN(blade_inner_upper) || double.IsNaN(blade_inner_lower))
					return double.NaN;
				return blade_inner_upper - blade_inner_lower;
			}
		}
        public double blade_outer_width
		{
			get
			{
				if (double.IsNaN(blade_outer_upper) || double.IsNaN(blade_outer_lower))
					return double.NaN;
				return blade_outer_upper - blade_outer_lower;
			}
		}

        public double getCalibrateLength(string id)
        {
            if(id=="full_length")
            {
                if (double.IsNaN(this.full_Length))
                    return double.NaN;
                return this.calibratedLength(this.full_Length);
            } 
            else if (id == "handle_width")
            {
                if (double.IsNaN(this.handle_Width))
                    return double.NaN;
                return this.calibratedLength(this.handle_Width);
            }
            else if (id == "handle_length")
            {
                if (double.IsNaN(this.handle_Length))
                    return double.NaN;
                return this.calibratedLength(this.handle_Length);
            }
            else if (id == "blade_width")
            {
                if (double.IsNaN(this.blade_inner_width))
                    return double.NaN;
				return this.calibratedLength(blade_inner_width);
            }
            else if (id == "blade_outer_width")
            {
                if (double.IsNaN(this.blade_outer_width))
                    return double.NaN;
                return this.calibratedLength(this.blade_outer_width);
            }
            throw new Exception("id not exist");
        }

		private double calibratedLength(double len)
		{
			return this.calibrationInterception + (len*calibrationFactor);
		}
        //mearsure border
        public int borderWidth;

        //for debug
        public Point[] cal1_leftUps=null;
        public Point[] cal1_rightUps=null;
        public Point[] cal1_leftDowns = null;
        public Point[] cal1_rightDowns = null;

        private millingCutterMeasurer(Bitmap im, int seekBorder, double factor=1, double intercept=0,  double cannyThr =160.0, double cannyLink = 40.0)
        {
            this.calibrationFactor = factor;
			this.calibrationInterception = intercept;
            this.sourceImage = im;
            this.cannyThreshold = cannyThr;
            this.cannyLinking = cannyLink;
            this.borderWidth = seekBorder;

            this.doEdgeCV(cannyThr, cannyLink);
        }

        //using holder to calibrate image directoin
        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="distance"></param>
        /// <param name="sloopLimit"></param>
        /// <param name="parallelismLimit">in degree</param>
        /// <returns>is sloop and parallelism valid or not</returns>
        public bool calibrateBase(Rectangle box, int distance, double sloopLimit=5.0, double parallelismLimit=2.0)
        {
            if (box == null || distance <box.Height+1 )
                throwException("Invalide calibration box");

            if (this.orignalEdge == null)
                return false;

            int centerLine=box.X+(box.Width/2);
            List<Point> LeftUpEdgePoints = new List<Point>();
            List<Point> RightUpEdgePoints = new List<Point>();
            List<Point> LeftDownEdgePoints = new List<Point>();
            List<Point> RightDownEdgePoints = new List<Point>();

            int xEnd=box.X+box.Width-1;

            for (int y = 0; y < box.Height; ++y)
            {
                int yp=y+box.Y;
                for(int x=centerLine; x>0; --x) //left
                {
                    //append box1
                    if (this.orignalEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        LeftUpEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                for (int x = centerLine; x < xEnd; ++x) //Right
                {
                    //append box2
                    if (this.orignalEdge.Data[yp , x, 0] > EDGE_Threshold)
                    {
                        RightUpEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                yp += distance;
                for (int x = centerLine; x > 0; --x) //left
                {
                    if (this.orignalEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        LeftDownEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                for (int x = centerLine; x < xEnd; ++x) //Right
                {
                    if (this.orignalEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        RightDownEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }
            }
#if DEBUG
            this.cal1_leftUps = LeftUpEdgePoints.ToArray();
            this.cal1_rightUps = RightUpEdgePoints.ToArray();
            this.cal1_leftDowns = LeftDownEdgePoints.ToArray();
            this.cal1_rightDowns = RightDownEdgePoints.ToArray();
#endif

            LeftUpEdgePoints.AddRange(LeftDownEdgePoints);
            RightUpEdgePoints.AddRange(RightDownEdgePoints);

            double intceptL, intceptR;
            double sloopL, sloopR;
            double rsL, rsR;

            /*
            if (!statistics.LinearRegression(LeftUpEdgePoints.ToArray(), out rsL, out intceptL, out sloopL))
                throwException("Regression Failed");
            if(!statistics.LinearRegression(RightUpEdgePoints.ToArray(), out rsR, out intceptR, out sloopR))
                throwException("Regression Failed");

            //this.leftIntercept = intceptL;
            //this.rightIntercept = intceptR;

            double angL = (Math.Atan(sloopL) * 360 / 2 / Math.PI);
            double angR = (Math.Atan(sloopR) * 360 / 2 / Math.PI);
            /**/

            if (!statistics.LinearRegression(LeftUpEdgePoints.ToArray(), out rsL, out intceptL, out sloopL,true))
                throwException("Regression Failed");
            if (!statistics.LinearRegression(RightUpEdgePoints.ToArray(), out rsR, out intceptR, out sloopR,true))
                throwException("Regression Failed");

            double angL = 90 - (Math.Atan(sloopL) * 360 / 2 / Math.PI);
            double angR = 90 - (Math.Atan(sloopR) * 360 / 2 / Math.PI);

			this.baseAngDiff = Math.Abs(Math.Abs(angL) - Math.Abs(angR));
            //this.baseSloop=(sloopL + sloopR) / 2;

            if (this.baseAngDiff > parallelismLimit || Math.Abs(Math.Abs((angL+angR)/2)-90) > sloopLimit)
                return false;

            try
            {
				//double ang = sloop2Angle(this.baseSloop);
                double ang = (angL + angR) / 2;
				this.baseCalibBlurImage = this.blurImage.Rotate((90 - Math.Abs(ang)) * ((ang>=0) ? 1 : -1 ), new Gray(0));
				
                
                this.baseCalibEdge = new Image<Gray, byte>(this.baseCalibBlurImage.Size);
                CvInvoke.Canny(this.baseCalibBlurImage, this.baseCalibEdge, this.cannyThreshold, this.cannyLinking);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 檢查銑刀並校正方向
        /// </summary>
        /// <param name="box"></param>
        /// <param name="distance"></param>
        /// <param name="sloopLimit"></param>
        /// <returns></returns>
        public bool calibrateCutter(Rectangle box, int distance, int gap, int searchWidth)
        {
            if (box == null || distance < box.Height + 1)
                throwException("Invalide calibration box");

            if (this.baseCalibEdge == null)
                return false;

            int centerLine = box.X + (box.Width / 2);
            List<Point> LeftEdgePoints = new List<Point>();
            List<Point> RightEdgePoints = new List<Point>();

            int xEnd = box.X + box.Width - 1;

            for (int y = 0; y < box.Height; ++y)
            {
                int yp = y + box.Y;
                for (int x = centerLine; x > 0; --x) //left
                {
                    //append box1
                    if (this.baseCalibEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        LeftEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                for (int x = centerLine; x < xEnd; ++x) //Right
                {
                    //append box2
                    if (this.baseCalibEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        RightEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                yp += distance;
                for (int x = centerLine; x > 0; --x) //left
                {
                    if (this.baseCalibEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        LeftEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }

                for (int x = centerLine; x < xEnd; ++x) //Right
                {
                    if (this.baseCalibEdge.Data[yp, x, 0] > EDGE_Threshold)
                    {
                        RightEdgePoints.Add(new Point(x, yp));
                        break;
                    }
                }
            }

            double intceptL, intceptR;
            double sloopL, sloopR;
            double rsL, rsR;

            //if (!statistics.LinearRegression(LeftEdgePoints.ToArray(), out rsL, out intceptL, out sloopL))
            //    throwException("Regression Failed");
            //if (!statistics.LinearRegression(RightEdgePoints.ToArray(), out rsR, out intceptR, out sloopR))
            //    throwException("Regression Failed");
            //get Left edge x, get right edge x
            //this.baseLeftEdgeX = (box.Y + box.Height + (distance / 2) - intceptL) / sloopL;
            //this.baseRightEdgeX = (box.Y + box.Height + (distance / 2) - intceptR) / sloopR;

            if (!statistics.LinearRegression(LeftEdgePoints.ToArray(), out rsL, out intceptL, out sloopL, true))
                throwException("Regression Failed");
            if (!statistics.LinearRegression(RightEdgePoints.ToArray(), out rsR, out intceptR, out sloopR, true))
                throwException("Regression Failed");

            this.baseLeftEdgeX = (box.Y + box.Height + (distance / 2)) * sloopL + intceptL;
            this.baseRightEdgeX = (box.Y + box.Height + (distance / 2)) * sloopR + intceptR;


            int checkLeftEdgeX = (int)Math.Abs(this.baseLeftEdgeX) - gap;
            int checkRightEdgeX = (int)Math.Abs(this.baseRightEdgeX) + gap;

            List<Point> cutterEdges = new List<Point>();
            int yCenter = this.baseCalibEdge.Height / 2;
            int searchHeight = (distance - box.Height) / 2;

            for (int i = 0; i < searchWidth; ++i)
            {
                int x = checkLeftEdgeX - i;
                for (int y = 0; y < searchHeight; ++y)
                {
                    if (this.baseCalibEdge.Data[yCenter - y, x, 0] > EDGE_Threshold)
                    {
                        cutterEdges.Add(new Point(x, yCenter - y));
                        break;
                    }
                }

                for (int y = 0; y < searchHeight; ++y)
                {
                    if (this.baseCalibEdge.Data[yCenter + y, x, 0] > EDGE_Threshold)
                    {
                        cutterEdges.Add(new Point(x, yCenter + y));
                        break;
                    }
                }

                x = checkRightEdgeX + i;
                for (int y = 0; y < searchHeight; ++y)
                {
                    if (this.baseCalibEdge.Data[yCenter - y, x, 0] > EDGE_Threshold)
                    {
                        cutterEdges.Add(new Point(x, yCenter - y));
                        break;
                    }
                }

                for (int y = 0; y < searchHeight; ++y)
                {
                    if (this.baseCalibEdge.Data[yCenter + y, x, 0] > EDGE_Threshold)
                    {
                        cutterEdges.Add(new Point(x, yCenter + y));
                        break;
                    }
                }

            }



            if (cutterEdges.Count < (3 * searchWidth))
                return false; //no cuterr or cutter too short

            double cutIntcept, cutSloop, cutRs;

            if (!statistics.LinearRegression(cutterEdges.ToArray(), out cutRs, out cutIntcept, out cutSloop))
                throwException("Regression Failed");

            this.cutterHandleSloop = cutSloop;

            try
            {
                this.cutterCalibBlurImage = this.baseCalibBlurImage.Rotate(-1 * sloop2Angle(this.cutterHandleSloop), new Gray(0));
                this.cutterCalibEdge = new Image<Gray, byte>(this.cutterCalibBlurImage.Size);
                CvInvoke.Canny(this.cutterCalibBlurImage, this.cutterCalibEdge, this.cannyThreshold, this.cannyLinking);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool doMeasure(int baseGap, int searchWidth, int searchHeight, int blade_chk_shift = 100, int blade_chk_width = 100, double handleSloopCutoff=0.2, int flank=5 )
        {
            //cal. cutter center line
            double intc, sloop;
            this.doMeasureCenter(this.cutterCalibEdge, baseGap, searchWidth, searchHeight, out intc, out sloop);
            this.cutterCenter =(int) Math.Round(intc);

            //find handle end
            this.handle_end = this.findHandleEnd(this.cutterCalibEdge, searchWidth, searchHeight);

            //find handl blade end
            int[] uppers = new int[this.cutterCalibEdge.Width];
            int[] lowers = new int[this.cutterCalibEdge.Width];
            for (int i = 0; i < uppers.Length; ++i )
            {
                uppers[i] = -1;
                lowers[i] = -1;
            }
            
            this.blade_end = this.findBladeEnd(this.cutterCalibEdge, searchWidth, searchHeight, uppers, lowers);

            this.full_Length = this.handle_end - this.blade_end;

            if((this.measureLeftEdgeX-this.blade_end)<100) //空針 or 超短斷針
            {
                this.handle_Width = Double.NaN;
                return false;
            }

            //handle width
            if (Double.IsNaN(this.handle_Width_R) || Double.IsInfinity(this.handle_Width_R) || Double.IsNaN(this.handle_Width_L) || Double.IsInfinity(this.handle_Width_L))
            {
                this.handle_Width = Double.NaN;
                return false;
            }
            else
            {
				//取較小值
				//this.handle_Width = (this.handle_Width_R > this.handle_Width_L) ? this.handle_Width_R : this.handle_Width_L;
				//取左區
				this.handle_Width = this.handle_Width_L;
			}

            //find handle start
            double[] smoothUppers = statistics.smoothByFlank(uppers, (int)this.blade_end, this.measureLeftEdgeX, flank);
            double[] smoothLowers = statistics.smoothByFlank(lowers, (int)this.blade_end, this.measureLeftEdgeX, flank);

            double[] thinkness = new double[smoothUppers.Length];
            for (int i = 0; i < thinkness.Length; ++i) thinkness[i] = -1;
            for (int i = (int)this.blade_end; i < this.measureLeftEdgeX; ++i)
                thinkness[i] = smoothLowers[i] - smoothUppers[i];

            double[] thinknessSloop = statistics.sloopByFlank(thinkness, (int)this.blade_end, this.measureLeftEdgeX, flank);

            for (int i = this.measureLeftEdgeX - flank - flank; i > this.blade_end; --i)
                if (thinknessSloop[i] >= handleSloopCutoff)
                {
                    this.handle_start = i;
                    break;
                }

			int sizeExt = baseGap / 4;
            int tStart=this.findHandleStart(this.grayImage, new Rectangle((int) this.handle_start + sizeExt, (int) (this.cutterCenter-this.handle_Width), (int) (this.measureLeftEdgeX-this.handle_start)+ sizeExt,(int) (this.handle_Width*2)),3,-0.04);
			if (tStart > sizeExt)
				this.handle_start += tStart - sizeExt;
           
            //find inner width
            //blade end - 2mm (2*0.18/0.00345 ~ 104px)
            //check len = 3mm (3*0.18/0.00345um ~ 156px)
            this.blade_width_check_start = (int) this.blade_end + blade_chk_shift;
            this.blade_width_check_end = (int)this.blade_width_check_start + blade_chk_width;

            this.doMeasureBlade(this.cutterCalibEdge, searchHeight);

            return true;
        }

        private int findHandleStart(Image<Gray, byte> oriImg, Rectangle imgBox, int scale=3, double cutOff=-0.03)
        {
            Image<Gray,byte> timage= new Image<Gray, byte>(imgBox.Size);
            CvInvoke.cvSetImageROI(this.grayImage, imgBox);
            CvInvoke.cvCopy(this.grayImage, timage, IntPtr.Zero);

            this.handleImage = new Image<Gray, byte>(imgBox.Width, imgBox.Height * scale);
			this.handleImage = timage.Resize(imgBox.Width, imgBox.Height * scale, Inter.Cubic);//.SmoothBlur(3,3);
            this.handleEdge = new Image<Gray, byte>(this.handleImage.Size);
            CvInvoke.Canny(this.handleImage, this.handleEdge, 120, 40, 3);

            //由右到左掃柄寛度
            this.handleEdgeHeights=new int[this.handleEdge.Width];
            int center = this.handleEdge.Height / 2;
            for(int x=0; x<this.handleEdgeHeights.Length; ++x)
            {
                int up, down;

				if (x == 177)
					up = 0;
                //toUp
                for(up=center; up>=0; --up)
                    if (this.handleEdge.Data[up,x, 0] > EDGE_Threshold)
                        break;

                //toDown
                for (down = center; down <this.handleEdge.Height; ++down)
                    if (this.handleEdge.Data[down,x, 0] > EDGE_Threshold)
                        break;

                this.handleEdgeHeights[x] = (up <= 0 || down >= (this.handleEdge.Height - 1)) ? -1 : down - up;
            }

			this.handleEdgeSmoothHeights = new double[this.handleEdgeHeights.Length];
			this.handleEdgeSmoothHeights=statistics.smoothByFlank(this.handleEdgeHeights, 0, this.handleEdgeHeights.Length, 13);
			/*/
			this.handleEdgeSloops= statistics.sloopByFlank(
				statistics.sloopByFlank(this.handleEdgeSmoothHeights, 0, this.handleEdgeHeights.Length-1, 11),
				0, this.handleEdgeHeights.Length - 1,23);
			/*/

			this.handleEdgeSloops = statistics.stageSloops(this.handleEdgeSmoothHeights, 0, this.handleEdgeHeights.Length - 1, 11);

			bool inPeak = false;
			for (int i = this.handleEdgeSloops.Length - 1; i > 0; --i)
			{
				if (!inPeak && this.handleEdgeSloops[i] < cutOff)
				{
					inPeak = true;
				}
				else if (inPeak && this.handleEdgeSloops[i] > cutOff)
					return i;
			}

			return 0;
		}




		private bool doMeasureBlade(Image<Gray,byte> edgeMap,int height)
        {

            for (int i = 0; i < height; ++i)
            {
                if (Double.IsNaN(this.blade_inner_upper))
                {
                    if(horizonHasEdge(edgeMap,this.cutterCenter+i,this.blade_width_check_start,this.blade_width_check_end))
                        this.blade_inner_upper = this.cutterCenter + i;
                }
                else if(Double.IsNaN(this.blade_outer_upper))
                {
                    if (!horizonHasEdge(edgeMap, this.cutterCenter + i, this.blade_width_check_start, this.blade_width_check_end))
                    {
                        this.blade_outer_upper = this.cutterCenter + i;
                        break;
                    }
                }
            }

            for (int i = 0; i < height; ++i)
            {
                if (Double.IsNaN(this.blade_inner_lower))
                {
                    if (horizonHasEdge(edgeMap, this.cutterCenter - i, this.blade_width_check_start, this.blade_width_check_end))
                        this.blade_inner_lower = this.cutterCenter - i;
                }
                else if (Double.IsNaN(this.blade_outer_lower))
                {
                    if (!horizonHasEdge(edgeMap, this.cutterCenter - i, this.blade_width_check_start, this.blade_width_check_end))
                    {
                        this.blade_outer_lower = this.cutterCenter - i;
                        break;
                    }
                }
            }

            if(Double.IsNaN(this.blade_inner_upper) || Double.IsNaN(this.blade_outer_upper) || Double.IsNaN(this.blade_inner_lower) || Double.IsNaN(this.blade_outer_lower))
            {
                return false;
            }

            return true;
        }

        private bool horizonHasEdge(Image<Gray, byte> edgeMap, int y, int xStart, int xEnd)
        {
            for (int x = xStart; x <= xEnd; ++x)
            {
                if(edgeMap.Data[y,x,0]>EDGE_Threshold)
                    return true;
            }

            return false;
        }

        //找刀右端點
        private double findHandleEnd(Image<Gray, byte> edgeMap, int searchWidth, int searchHeight)
        {
            int yStart = this.measureRightEdgeX;
            int yEnd = edgeMap.Width - this.borderWidth;
            int yWidthEnd = yStart + searchWidth;
            if (yWidthEnd > yEnd) yWidthEnd = yEnd;

            int xStart = this.cutterCenter;

            List<int> heights = new List<int>();
            int lastY = -1;
            for(int y=yStart; y<=yEnd; ++y)
            {
                int upEdge = -1;
                int downEdge = -1;

                for (int i = 0; i < searchHeight; ++i)
                {
                    //find first edge point
                    int uX = xStart - i;
                    if (upEdge < 0 && edgeMap.Data[uX, y, 0] > EDGE_Threshold)
                        upEdge = uX;
                    int dX = xStart + i;
                    if (downEdge < 0 && edgeMap.Data[dX, y, 0] > EDGE_Threshold)
                        downEdge = dX;

                    if (upEdge > 0 && downEdge > 0 && y<yWidthEnd) //上下有邊 且在採樣區內
                    {
                        heights.Add(downEdge - upEdge + 1);
                        break;
                    }
                }
                if (upEdge < 0 && downEdge < 0)  //上下無邊
                    break;

                lastY = y;
            }
            

            if (heights.Count< searchWidth * 0.5)
                return double.NaN;  //無物

            

            int[] handleWidths = heights.ToArray();
            this.handle_Width_R = (double)handleWidths.Sum() / (double)handleWidths.Length;

            if (lastY == yEnd) //found no end
            {
                return double.PositiveInfinity;
            }
            else
                return lastY;
        }

		//找刃邊端
        private double findBladeEnd(Image<Gray, byte> edgeMap, int searchWidth, int searchHeight, int[] uppers, int[] lowers)
        {
            int yStart = this.measureLeftEdgeX;
            int yEnd = this.borderWidth; //front

            int yWidthEnd = yStart - searchWidth;
            if (yWidthEnd < yEnd) yWidthEnd = yEnd;

            int xStart = this.cutterCenter;

            List<int> heights = new List<int>();
            int lastY = -1;
            for (int y = yStart; y >= yEnd; --y)
            {
                int upEdge = -1;
                int downEdge = -1;

                for (int i = 0; i < searchHeight; ++i)
                {
                    //find first edge point
                    int uX = xStart - i;
                    if (upEdge < 0 && edgeMap.Data[uX, y, 0] > EDGE_Threshold)
                    {
                        upEdge = uX;
                        uppers[y] = uX;
                    }
                    int dX = xStart + i;
                    if (downEdge < 0 && edgeMap.Data[dX, y, 0] > EDGE_Threshold)
                    {
                        downEdge = dX;
                        lowers[y] = dX;
                    }
                        

                    if (upEdge > 0 && downEdge > 0 && y > yWidthEnd) //上下有邊 且在採樣區內
                    {
                        heights.Add(downEdge - upEdge + 1);
                        break;
                    }
                }
                if (upEdge < 0 && downEdge < 0)  //上下無邊
                    break;

                lastY = y;
            }

            if (heights.Count < searchWidth * 0.5)
                return double.NaN;

            int[] handleWidths = heights.ToArray();
            this.handle_Width_L = (double)handleWidths.Sum() / (double)handleWidths.Length;
            if(lastY==yEnd)
            {
                return Double.PositiveInfinity;
            }
            else
                return lastY;
        }

        private void  doMeasureCenter(Image<Gray,byte> edgeMap, int baseGap, int searchWidth, int searchHeight, out double intercept, out double sloop)
        {
            List<Point> upEdges = new List<Point>();
            List<Point> downEdges = new List<Point>();

            int centerLineY = edgeMap.Height / 2;

            this.measureLeftEdgeX= (int)Math.Abs(this.baseLeftEdgeX) - baseGap;
            this.measureRightEdgeX = (int)Math.Abs(this.baseRightEdgeX) + baseGap;

            for (int i = 0; i < searchWidth; ++i)
            {
                int x = this.measureLeftEdgeX - i;
                for (int y = 0; y < searchHeight; ++y)
                {
                    if (edgeMap.Data[centerLineY - y, x, 0] > EDGE_Threshold)
                    {
                        upEdges.Add(new Point(x, centerLineY - y));
                        break;
                    }
                }

                for (int y = 0; y < searchHeight; ++y)
                {
                    if (edgeMap.Data[centerLineY + y, x, 0] > EDGE_Threshold)
                    {
                        downEdges.Add(new Point(x, centerLineY + y));
                        break;
                    }
                }

                x = this.measureRightEdgeX + i;
                for (int y = 0; y < searchHeight; ++y)
                {
                    if (edgeMap.Data[centerLineY - y, x, 0] > EDGE_Threshold)
                    {
                        upEdges.Add(new Point(x, centerLineY - y));
                        break;
                    }
                }

                for (int y = 0; y < searchHeight; ++y)
                {
                    if (edgeMap.Data[centerLineY + y, x, 0] > EDGE_Threshold)
                    {
                        downEdges.Add(new Point(x, centerLineY + y));
                        break;
                    }
                }

            }

            double upRs, upIntc, upSloop;
            if (!statistics.LinearRegression(upEdges.ToArray(), out upRs, out upIntc, out upSloop))
                throwException("Regression Failed");
            double lowRs, lowIntc, lowSloop;
            if (!statistics.LinearRegression(downEdges.ToArray(), out lowRs, out lowIntc, out lowSloop))
                throwException("Regression Failed");

            double angU = Math.Atan(upSloop) * 180 / Math.PI;
            double angD = Math.Atan(lowSloop) * 180 / Math.PI;

            this.handle_parallelism = angU - angD;

            List<Point> cutterEdges = new List<Point>();
            cutterEdges.AddRange(upEdges);
            cutterEdges.AddRange(downEdges);

            double rs;
            if (!statistics.LinearRegression(cutterEdges.ToArray(), out rs, out intercept, out sloop))
                throwException("Regression Failed");

        }

        public static millingCutterMeasurer Create(string file, out string message, int seekBorder, double? intercept=null, double? factor=null)
        {
            if(file==null || !File.Exists(file))
            {
                message = "file is not exist";
                return null;
            }

            Bitmap im;

            try
            {
                im = new Bitmap(file);
            }
            catch
            {
                message = "file is invalid image file";
                return null;
            }

            if (im == null)
            {
                message = "Null Image";
                return null;
            }
            if(im.Width!=2448 || im.Height!=2048)
            {
                message = "Size mismatch";
                return null;
            }

            if(factor!=null && (factor<0 || Double.IsNaN((double) factor) || Double.IsInfinity((double) factor)))
            {
                message = "Invalid calibration factor";
                return null;
            }

			if(intercept!=null && (double.IsNaN((double)intercept) || double.IsInfinity((double) intercept)))
			{
				message = "Invalid calibration intercept";
				return null;
			}

            message = "";

            return new millingCutterMeasurer(im, seekBorder,intercept.GetValueOrDefault(0), factor.GetValueOrDefault(1));

        }

        public static millingCutterMeasurer Create(Bitmap im, out string message, int seekBorder, double? intercept=null, double? factor = null)
        {
            if (im == null)
            {
                message = "Null Image";
                return null;
            }
            if (im.Width != 2448 || im.Height != 2048)
            {
                message = "Size mismatch";
                return null;
            }

            if (factor != null && (factor < 0 || Double.IsNaN((double)factor) || Double.IsInfinity((double)factor)))
            {
                message = "Invalid calibration factor";
                return null;
            }

            message = "";
            return new millingCutterMeasurer(im, seekBorder, factor.GetValueOrDefault(1),intercept.GetValueOrDefault(0));

        }

        private void doEdgeCV(double cannyThreshold, double cannyLinking)
        {
            Image<Bgr, Byte> ori = new Image<Bgr, byte>(this.sourceImage);
            this.grayImage = new Image<Gray, byte>(ori.Size);
            CvInvoke.CvtColor(ori, grayImage, ColorConversion.Bgr2Gray);

           // Mat pyr =new Mat();
            //CvInvoke.PyrDown(grayImage, pyr);
            this.blurImage = new Image<Gray, byte>(this.grayImage.Size);
            //CvInvoke.PyrUp(pyr, this.blurImage);
            this.blurImage = new Image<Gray, byte>(this.grayImage.Bitmap);
            //CvInvoke.GaussianBlur(this.grayImage, this.blurImage,new Size(3,3), 0.5);



            this.orignalEdge = new Image<Gray, byte>(this.blurImage.Size);
            CvInvoke.Canny(this.grayImage, orignalEdge, cannyThreshold, cannyLinking);
        }

        
        //Tools
        public double sloop2Angle(double sloop)
        {
            return Math.Atan(sloop) / Math.PI * 180;
        }
        /// <summary>
        /// 例外擲出器
        /// </summary>
        /// <param name="message"></param>
        /// <param name="functionName"></param>
        public void throwException(string message, int? ExceptionID = null)
        {
            string msg = this.GetType().Name;
            msg += "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
            msg += ":" + message;
            throw new Exception(msg);
        }
    }
}

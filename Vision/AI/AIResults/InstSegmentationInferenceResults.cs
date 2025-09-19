using VisionNet.Vision.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using VisionNet.Image;

namespace VisionNet.Vision.AI
{
    public class InstSegmentationInferenceResults : RemoteClientInferenceResult
    {
        public List<SingleInstSegmentationInferenceResult> Instances { get; set; } = new List<SingleInstSegmentationInferenceResult>();
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        [JsonIgnore]
        public bool HasResult => Instances?.Count > 0;

        [JsonIgnore]
        public int NumInstances => Instances?.Count ?? 0;

        public static InstSegmentationInferenceResults Default => new InstSegmentationInferenceResults
        {
            Id = new Guid("5f3723fb-5768-458e-a118-eb69b9821e39"),
            Success = true,
            Error = "None",
            ImageWidth = 200,
            ImageHeight = 200,
            ProcessingTime = 0.1,
            Instances = new List<SingleInstSegmentationInferenceResult>
            {
                new SingleInstSegmentationInferenceResult
                {
                    ClassIndex = 1,
                    ClassName = "Tipo1",
                    Score = 0.4,
                    Center = new PointF {
                            X = 150.89F,
                            Y = 250.45F
                          },
                    BoundingBox = new BoundingBox{
                        X = 100.55F,
                        Y = 80.69F,
                        Width = 50.58F,
                        Height = 15.25F,
                        Angle = 47.23F
                    },
                    Polygon = new Polygon()
                        .AddRange(
                            new PointF[]
                            {
                                new PointF{
                                    X = 31.25F,
                                    Y = 104.87F
                                },
                                new PointF{
                                    X = 63.87F,
                                    Y = 97.15F
                                },
                                new PointF{
                                    X = 101.98F,
                                    Y = 12.58F
                                },
                                new PointF{
                                    X = 36.81F,
                                    Y = 42.91F
                                }
                            }),
                    Position = new CoodinateSystem
                    {
                        Origin = new PointF
                        {
                            X = 101.25F,
                            Y = 95.59F
                        },
                        Angle = 17.95
                    }
                },
                new SingleInstSegmentationInferenceResult
                {
                    ClassIndex = 1,
                    ClassName = "Tipo1",
                    Score = 0.4,
                    Center = new PointF {
                            X = 150.89F,
                            Y = 250.45F
                          },
                    BoundingBox = new BoundingBox{
                        X = 100.55F,
                        Y = 80.69F,
                        Width = 50.58F,
                        Height = 15.25F,
                        Angle = 47.23F
                    },
                    Polygon = new Polygon()
                        .AddRange(
                        new PointF[]
                            {
                            new PointF{
                                X = 31.25F,
                                Y = 104.87F
                            },
                            new PointF{
                                X = 63.87F,
                                Y = 97.15F
                            },
                            new PointF{
                                X = 101.98F,
                                Y = 12.58F
                            },
                            new PointF{
                                X = 36.81F,
                                Y = 42.91F
                            }
                        }
                    ),
                    Position = new CoodinateSystem
                    {
                        Origin = new PointF
                        {
                            X = 101.25F,
                            Y = 95.59F
                        },
                        Angle = 17.95
                    }
                },
                new SingleInstSegmentationInferenceResult
                {
                    ClassIndex = 1,
                    ClassName = "Tipo1",
                    Score = 0.4,
                    Center = new PointF {
                            X = 150.89F,
                            Y = 250.45F
                          },
                    BoundingBox = new BoundingBox{
                        X = 100.55F,
                        Y = 80.69F,
                        Width = 50.58F,
                        Height = 15.25F,
                        Angle = 47.23F
                    },
                    Polygon = new Polygon()
                        .AddRange(new PointF[]
                            {
                            new PointF{
                                X = 31.25F,
                                Y = 104.87F
                            },
                            new PointF{
                                X = 63.87F,
                                Y = 97.15F
                            },
                            new PointF{
                                X = 101.98F,
                                Y = 12.58F
                            },
                            new PointF{
                                X = 36.81F,
                                Y = 42.91F
                            }
                        }
                    ),
                    Position = new CoodinateSystem
                    {
                        Origin = new PointF
                        {
                            X = 101.25F,
                            Y = 95.59F
                        },
                        Angle = 17.95
                    }
                }
            }
        };
    }
}

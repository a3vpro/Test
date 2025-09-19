//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 01-06-2020
//
// Last Modified By : aibanez
// Last Modified On : 29-09-2020
// Description      : v1.4.2
//
// Copyright        : (C)  2020 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

using Conditions;
using VisionNet.Vision.Core;
using System.Collections.Generic;
using VisionNet.Image;
using System.Linq;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using VisionNet.Conditions.Paths;
using VisionNet.IO.Paths;
using VisionNet.Core.Types;
using System;
using VisionNet.Image.VisionPro;
using System.Collections;
using VisionNet.Core.Enums;

namespace VisionNet.Vision.VisionPro
{
	public abstract class VisionProVisionFunctionBase<TOptions, TInputParam, TOutputParam> : VisionFunction<TOptions, TInputParam, TOutputParam>, IVisionFunction
		where TOptions : VisionFunctionOptions, new()
		where TInputParam : InputParametersCollection, new ()
		where TOutputParam : OutputParametersCollection, new ()
	{
		/// <summary>
		/// Variable CogToolBlock que carga la aplicación
		/// </summary>
		protected CogToolBlock _toolBlock;

		public override VisionFunctionType Type => VisionFunctionType.VisionProToolBlock;

		public new VisionProFunctionOptions Options
		{
			get => base.Options as VisionProFunctionOptions;
			set => base.Options = value;
		}

		public VisionProVisionFunctionBase() : base()
		{
		}

		protected override void _configure(VisionFunctionOptions options)
		{
			options.Requires(nameof(options)).IsOfType(typeof(VisionProFunctionOptions));
			var VisionProOptions = (VisionProFunctionOptions)options;

			var path = VisionProOptions.ToolBlockFilePath;
			path.Requires(nameof(VisionProOptions.ToolBlockFilePath))
				.IsNotNullOrWhiteSpace()
				.Evaluate(path.IsValidAbsoluteDirectoryPath()
				|| path.IsValidRelativeDirectoryPath());

			if (path.IsValidRelativeDirectoryPath())
				path = path.FromExeRelativePathToAbsolutePath();
			path.Requires(nameof(VisionProOptions.ToolBlockFilePath))
				.FileExists();

			// Multithreading
			CogVisionToolMultiThreading.ThreadCountMode = CogVisionToolMultiThreadingThreadCountModeConstants.HardwareDefined;
			CogVisionToolMultiThreading.Enable = true;

			// Se carga el job
			object vppFile = CogSerializer.LoadObjectFromFile(path);

			// Se valida que el job sea del tipo CogToolBlock
			vppFile.Ensures(nameof(VisionProOptions.ToolBlockFilePath))
				.IsOfType(typeof(CogToolBlock));

			_toolBlock = (CogToolBlock)vppFile;

			// Asignación de parámetros de entrada de inicialización. 
			// TODO: Revisar si es necesario!!
			var executionInRuntimeInputParams = _inputParameters.GetAll()
				.Where(p =>
				p.Source == ParameterSource.Constant &&
				p.Scope == ParameterScope.Initialization);
			foreach (var inputParam in executionInRuntimeInputParams)
			{
				var index = inputParam.ExternalIndex;
				var type = inputParam.DataType;
				var isArray = inputParam.IsArray;
				var value = inputParam.Value;

				SetInput(_toolBlock, index, type, isArray, value);
			}
			Options = VisionProOptions;
		}
		
		protected override void _initialize()
		{
			// Asignación de parámetros de entrada de inicialización
			var executionInRuntimeInputParams = _inputParameters.GetAll()
			.Where(p =>
			//p.Source == ParameterSource.Constant && // Los contantes se deben enviar al configurar la función de visión
			p.Scope == ParameterScope.Initialization);

			foreach (var inputParam in executionInRuntimeInputParams)
			{
				var index = inputParam.ExternalIndex;
				if (index != "Initialize")
				{
					var type = inputParam.DataType;
					var isArray = inputParam.IsArray;
					var value = inputParam.Value;

					SetInput(_toolBlock, index, type, isArray, value);
				}
			}

			if (ExistsInput(_toolBlock, "Initialize"))
			{
				SetInput(_toolBlock, "Initialize", BasicTypeCode.Boolean, false, true);

				_toolBlock.Run();

				SetInput(_toolBlock, "Initialize", BasicTypeCode.Boolean, false, false);
			}
		}		

		protected override bool _execute(InputParametersCollection inputParams, ref OutputParametersCollection outputParams)
		{
			bool success = true;

			// Asignación de parámetros de entrada
			var executionInRuntimeInputParams = inputParams.GetAll()
				.Where(p =>
				//p.Source == ParameterSource.Runtime &&
				p.Scope == ParameterScope.Execution);
			foreach (var inputParam in executionInRuntimeInputParams)
			{
				var index = inputParam.ExternalIndex;
				var type = inputParam.DataType;
				var isArray = inputParam.IsArray;
				var value = inputParam.Value;

				SetInput(_toolBlock, index, type, isArray, value);
			}

			// Ejecución
			_toolBlock.Run();

			// Consulta de parámetros de salida
			var executionInRuntimeOutputParams = outputParams.GetAll()
							.Where(p =>
							p.Source == ParameterSource.Runtime &&
							p.Scope == ParameterScope.Execution);
			foreach (var outputParam in executionInRuntimeOutputParams)
			{
				var index = outputParam.ExternalIndex;
				var type = outputParam.DataType;
				var isArray = outputParam.IsArray;
				
				var successRead = GetOutput(_toolBlock, index, type, isArray, out var value);

				outputParams.TrySetValue(index, value);
				success &= successRead;
			}

			return success;
		}

		protected bool ExistsInput(CogToolBlock _toolBlock, string paramName)
		{
			return _toolBlock.Inputs.Contains(paramName);
		}

		protected bool ExistsOutput(CogToolBlock _toolBlock, string paramName)
		{
			return _toolBlock.Outputs.Contains(paramName);
		}

		protected void SetInput(CogToolBlock _toolBlock, string paramName, BasicTypeCode dataType, bool isArray, object value)
		{
			// Guards
			value.Requires(paramName).IsNotNull();
			dataType.Requires(paramName).Evaluate(dataType.IsIn(BasicTypeCode.IntegerNumber, BasicTypeCode.FloatingPointNumber, BasicTypeCode.String, BasicTypeCode.Boolean, BasicTypeCode.DateTime, BasicTypeCode.Image, BasicTypeCode.Object));
			if (!isArray && dataType == BasicTypeCode.Image)
				value.Requires(paramName).IsOfType(typeof(IImage));
			if (isArray)
				value.Requires(paramName).IsOfType(typeof(IList));
			if (isArray && dataType == BasicTypeCode.Image)
				foreach (var item in value as IList)
					item.Requires(paramName).IsOfType(typeof(IImage));

			switch (dataType)
			{
				case BasicTypeCode.IntegerNumber:
				case BasicTypeCode.Boolean:
				case BasicTypeCode.FloatingPointNumber:
				case BasicTypeCode.String:
				case BasicTypeCode.DateTime:
				case BasicTypeCode.Object:
					_toolBlock.Inputs[paramName].Value = value;
					break;
				case BasicTypeCode.Image:
					if (!isArray)
					{ 
						// Non array parameter assignement
						ImageVisionPro image;
						if (value is ImageVisionPro)
							image = value as ImageVisionPro;
						else
							image = (value as IImage).ConvertTo<ImageVisionPro>();
						_toolBlock.Inputs[paramName].Value = image.Target;
					}
					else
					{
						// Array parameter assignement
						var valueList = value as IList;
						var resultImageList = new List<ICogImage>();
						foreach (var imageValue in valueList)
						{
							ImageVisionPro image;
							if (imageValue is ImageVisionPro)
								image = imageValue as ImageVisionPro;
							else
								image = (imageValue as IImage).ConvertTo<ImageVisionPro>();
							resultImageList.Add(image.Target);
						}
						_toolBlock.Inputs[paramName].Value = resultImageList;
					}
					break;
			};
		}

		private bool GetOutput(CogToolBlock _toolBlock, string paramName, BasicTypeCode dataType, bool isArray, out object value)
		{
			value = null;

			
			switch (dataType)
			{
				case BasicTypeCode.IntegerNumber:
				case BasicTypeCode.Boolean:
				case BasicTypeCode.FloatingPointNumber:
				case BasicTypeCode.String:
				case BasicTypeCode.DateTime:
				case BasicTypeCode.Object:
				default:
					value = _toolBlock.Outputs[paramName].Value;
					break;
				case BasicTypeCode.Image:
					if (!isArray)
					{
						// Non array parameter assignement
						object objValue = _toolBlock.Outputs[paramName].Value;

						objValue.Ensures(paramName)
							.IsOfType(typeof(ICogImage));

						value = new ImageVisionPro(objValue, Guid.NewGuid(), $"{Index}_{paramName}", $"Result image of parameter {paramName}");
					}
					else
					{
						// Array parameter assignement
						var imageList = new List<IImage>();

						object objValue = _toolBlock.Outputs[paramName].Value;
						var objList = objValue as IList<ICogImage>;
						var i = 0;
						foreach (var imageValue in objList)
						{
							var tmpValue = new ImageVisionPro(imageValue, Guid.NewGuid(), $"{Index}_{paramName}_{i}", $"Result image of parameter {paramName}");
							imageList.Add(tmpValue);
							i++;
						}
						value = imageList;
					}
					break;
			}

			// Guards
			value.Ensures(paramName).IsNotNull();
			dataType.Ensures(paramName).Evaluate(dataType.IsIn(BasicTypeCode.IntegerNumber, BasicTypeCode.FloatingPointNumber, BasicTypeCode.String, BasicTypeCode.Boolean, BasicTypeCode.DateTime, BasicTypeCode.Image, BasicTypeCode.Object));
			if (!isArray && dataType == BasicTypeCode.Image)
				value.Ensures(paramName).IsOfType(typeof(IImage));
			if (isArray)
				value.Ensures(paramName).IsOfType(typeof(IList));
			if (isArray && dataType == BasicTypeCode.Image)
				foreach (var item in value as IList)
					item.Ensures(paramName).IsOfType(typeof(IImage));

			return true;
		}
	}
}
using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using VisionNet.Core.Abstractions;
using VisionNet.Image;
using VisionNet.Core.Types;
using System.Collections;
using VisionNet.Core.SafeObjects;

namespace VisionNet.Vision.Core
{
	public class VisionMessage : IVisionMessage
	{
		/// <inheritdoc/>
		public string SystemSource { get; private set; }

		/// <inheritdoc/>
		public long Index { get; private set; }

		/// <inheritdoc/>
		public List<NamedValue> Features { get; private set; }

		/// <inheritdoc/>
		public List<NamedValue> Step { get; private set; }

		/// <inheritdoc/>
		public DateTime AcquisitionMoment { get; private set; }

		/// <inheritdoc/>
		public IImageCollection SourceImages { get; private set; }

		/// <inheritdoc/>
		public string PrevVisionFunctionName { get; private set; }

		/// <inheritdoc/>
		public IMessageParametersCollection PrevResults { get; private set; } = new OutputParametersCollection();

		/// <inheritdoc/>
		public List<IParameter> LstPipelineParameters { get; private set; } = new List<IParameter>();

		/// <summary>
		/// The .ctor.
		/// </summary>
		/// <param name="systemSource">The system source.</param>
		/// <param name="externalIndex">The piece index.</param>
		/// <param name="inspectionStep">The inspection step.</param>
		/// <param name="productFeatures">The features.</param>
		/// <param name="sourceImages">The source images.</param>
		public VisionMessage(string systemSource, long externalIndex, List<NamedValue> productFeatures, List<NamedValue> inspectionStep, IImageCollection sourceImages)
		{
			systemSource.Requires(nameof(systemSource)).IsNotNullOrWhiteSpace();
			externalIndex.Requires(nameof(externalIndex)).IsGreaterOrEqual(0);
			inspectionStep.Requires(nameof(inspectionStep)).IsNotNull();
			productFeatures.Requires(nameof(productFeatures)).IsNotNull();
			sourceImages.Requires(nameof(sourceImages))
				.IsNotNull()
				.Evaluate(ic => ic.Count > 0);

			SystemSource = systemSource;
			Index = externalIndex;
			Features = productFeatures;
			Step = new List<NamedValue>(inspectionStep);
			AcquisitionMoment = sourceImages.FirstOrDefault().Value.CreationTime;
			SourceImages = sourceImages;
			PrevVisionFunctionName = string.Empty;
			(PrevResults as OutputParametersCollection).PrevResult = true;
			(PrevResults as OutputParametersCollection).Success = true;
		}

		/// <summary>
		/// Clear the previous results in order to set the new one
		/// </summary>
		public VisionMessage(VisionMessage other, string visionFunctionName, List<NamedValue> step = null)
		{
			SystemSource = other.SystemSource;
			Index = other.Index;
			Features = other.Features;
			Step = step ?? (other.Step != null ? new List<NamedValue>(other.Step) : new List<NamedValue>());
			AcquisitionMoment = other.AcquisitionMoment;
			SourceImages = other.SourceImages;
			PrevVisionFunctionName = visionFunctionName;
			LstPipelineParameters = other.LstPipelineParameters != null
				? new List<IParameter>(other.LstPipelineParameters)
				: new List<IParameter>();
		}

		/// <summary>
		/// Clear the previous results in order to set the new one, including the new results
		/// </summary>
		public VisionMessage(VisionMessage other, string visionFunctionName, IOutputParametersCollection outputParameters, List<NamedValue> step = null)
		{
			SystemSource = other.SystemSource;
			Index = other.Index;
			Features = other.Features;
			Step = step ?? (other.Step != null ? new List<NamedValue>(other.Step) : new List<NamedValue>());
			AcquisitionMoment = other.AcquisitionMoment;
			SourceImages = other.SourceImages;
			PrevVisionFunctionName = visionFunctionName;
			UpdateResults(outputParameters);
			LstPipelineParameters = other.LstPipelineParameters != null
				? new List<IParameter>(other.LstPipelineParameters)
				: new List<IParameter>();
		}

		/// <summary>
		/// Update all the parameters with all the output parameters from the previous visión function
		/// </summary>
		/// <param name="visionFunctionName">Name of the previous vision function</param>
		/// <param name="parameters">The output parameters from the previous visión function</param>
		public void UpdateResults(IReadonlyParametersCollection parameters, List<string> excludeParamNames = null)
		{
			parameters.Requires(nameof(parameters))
				.IsNotNull();

			foreach (var parameter in parameters.GetAll())
			{
				bool include = true;
				if (excludeParamNames != null && excludeParamNames.Count > 0)
					include = !excludeParamNames.Any(n => string.Equals(n, parameter.Name, StringComparison.CurrentCultureIgnoreCase));

				if (include)
					if (PrevResults.Exists(parameter.Name))
						PrevResults.TrySetValue(parameter.Name, parameter.Value);
					else
						PrevResults.TryAdd(parameter);
			}
		}

		/// <summary>
		/// Update on Step with an extra NamedValue
		/// </summary>
		/// <param name="newStep">The NamedValue to add</param>
		public void AddStep(NamedValue newStep)
		{
			Step.Add(newStep);
		}

		/// <summary>
		/// Update on Step with an extra NamedValue
		/// </summary>
		/// <param name="name">The name of the NamedValue to add</param>
		/// <param name="type">The type of the NamedValue to add</param>
		/// <param name="value">The value of the NamedValue to add</param>
		public void AddStep(string name, BasicTypeCode type, object value)
		{
			Step.Add(new NamedValue
			{
				Name = name,
				Type = type,
				Value = value,
			});
		}

		/// <summary>
		/// Update on parameters with the output parameters from the previous visión function
		/// </summary>
		/// <param name="parameter">The parameter to add</param>
		public void AddParameter(Parameter parameter)
		{
			var param = parameter;

			var parameterClass = param.Clone();
			PrevResults.TryAdd(parameterClass);
		}

		/// <summary>
		/// Add a Parameter to LstPipelineParameters
		/// </summary>
		/// <param name="parameter">The parameter to add</param>
		public void AddPipelineParameter(IParameter parameter)
		{
			LstPipelineParameters.Add(parameter);
		}

		/// <summary>
		/// Add a Parameter to LstPipelineParameters from an Array Parameter
		/// </summary>
		/// <param name="parameter">The array parameter</param>
		/// <param name="index">The index from the array</param>
		/// <param name="newName">The new name to give the extracted parameter</param>
		public void AddPipelineParameterFromArray(Parameter parameter, int index, string newName = "")
		{
			index.Requires(nameof(index))
				.IsGreaterOrEqual(0);
			parameter.Requires(nameof(parameter))
				.IsNotNull();
			parameter.IsArray.Requires(nameof(parameter.IsArray))
				.IsTrue();
			parameter.Value.Requires(nameof(parameter.Value))
				.IsNotNull()
				.IsOfType(typeof(IList));

			var parameterClass = parameter.Clone();
			parameterClass.ParentName = PrevVisionFunctionName;
			if (newName != string.Empty)
			{
				parameterClass.Index = newName;
				parameterClass.ExternalIndex = newName;
				parameterClass.Name = newName;
				parameterClass.Description = $"Single instance from '{parameter.Description}'";
			}

			parameterClass.IsArray = false;

			var valueList = parameter.Value as IList;
			index.Requires(nameof(index))
				.IsLessThan(valueList.Count);

			parameterClass.TrySetValue((parameter.Value as IList)[index]);

			AddPipelineParameter(parameterClass);
		}


		/// <summary>
		/// Add a new instance of the <see cref="Parameter"/> class with specified properties.
		/// </summary>
		/// <param name="index">The index of the parameter.</param>
		/// <param name="externalIndex">The external reference index.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="description">The description of the parameter.</param>
		/// <param name="direction">The intended direction (input/output) of the parameter.</param>
		/// <param name="source">The source of the parameter value (fixed/runtime).</param>
		/// <param name="scope">The scope or intended use of the parameter.</param>
		/// <param name="dataType">The data type of the parameter.</param>
		/// <param name="isArray">Indicates if the parameter is an array.</param>
		/// <param name="defaultValue">The default value of the parameter.</param>
		/// <param name="preferences">Type conversion preferences.</param>
		public void AddParameter(string index, string externalIndex, string name, string description, ParameterDirection direction, ParameterSource source, ParameterScope scope, BasicTypeCode dataType, bool isArray, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
		{
			var parameter = new Parameter(index, externalIndex, name, description, direction, source, scope, dataType, isArray, defaultValue, preferences);
			PrevResults.TryAdd(parameter);
		}

		/// <summary>
		/// Update on parameters with the output parameters from the previous visión function
		/// </summary>
		/// <param name="currentName">The current name of the parameter</param>
		/// <param name="newName">The new name of the parameter</param>
		public void ChangeParameterName(string currentName, string newName)
		{
			var prevResults = PrevResults as ParametersCollection;

			prevResults.Requires(nameof(PrevResults))
				.Evaluate(p => p.Exists(currentName));
			var parameter = prevResults.Get(currentName) as Parameter;

			var parameterClass = parameter.Clone(); // TODO: ¿Por qué es necesario clonar?
			if (!string.IsNullOrWhiteSpace(newName))
			{
				parameterClass.Index = newName;
				parameterClass.ExternalIndex = newName;
				parameterClass.Name = newName;
			}

			PrevResults.TryAdd(parameterClass);
			PrevResults.TryRemove(currentName); // TODO: tal vez no sea necesario añadir para despues eliminar el anterior
		}

		/// <summary>
		/// Update on parameters with the output parameters from the previous visión function
		/// </summary>
		/// <param name="parameter">The parameter from the previous visión function</param>
		/// <param name="value">The new value of the parameter</param>
		public void ChangeValue(string parameterName, object value)
		{
			var prevResults = PrevResults as ParametersCollection;

			prevResults.Requires(nameof(PrevResults))
				.Evaluate(p => p.Exists(parameterName));

			prevResults.TrySetValue(parameterName, value);

			// Corrección de bug: Intentaba añadir pero no cambiar su valor.
			//var parameter = prevResults.Get(parameterName) as Parameter;
			//var parameterClass = parameter.Clone();
			//parameterClass.Value = value;

			//PrevResults.TryAdd(parameterClass);
		}

		/// <summary>
		/// Update on parameters with the output parameters from the previous visión function
		/// </summary>
		/// <param name="visionFunctionName">Name of the previous vision function</param>
		/// <param name="parameter">The parameter from the previous visión function</param>
		public void UpdateFromArray(Parameter parameter, int index, string newName = "")
		{
			index.Requires(nameof(index))
				.IsGreaterOrEqual(0);
			parameter.Requires(nameof(parameter))
				.IsNotNull();
			parameter.IsArray.Requires(nameof(parameter.IsArray))
				.IsTrue();
			parameter.Value.Requires(nameof(parameter.Value))
				.IsNotNull()
				.IsOfType(typeof(IList));

			var parameterClass = parameter.Clone();
			parameterClass.ParentName = PrevVisionFunctionName;
			if (newName != string.Empty)
			{
				parameterClass.Index = newName;
				parameterClass.ExternalIndex = newName;
				parameterClass.Name = newName;
				parameterClass.Description = $"Single instance from '{parameter.Description}'";
			}

			parameterClass.IsArray = false;

			var valueList = parameter.Value as IList;
			index.Requires(nameof(index))
				.IsLessThan(valueList.Count);

			parameterClass.TrySetValue((parameter.Value as IList)[index]);

			PrevResults.TryAdd(parameterClass);
		}
	}
}

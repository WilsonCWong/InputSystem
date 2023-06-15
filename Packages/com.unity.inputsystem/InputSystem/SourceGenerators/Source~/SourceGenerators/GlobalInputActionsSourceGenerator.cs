using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Unity.InputSystem.SourceGenerators
{
    [Generator]
    public class GlobalInputActionsSourceGenerator : ISourceGenerator
    {
        private const string GlobalActionsAssetPath = "Packages/com.unity.inputsystem/InputSystem/API/GlobalInputActions.inputactions";

        // Entry assembly is null in VS IDE
        // At build time, it is `csc` or `VBCSCompiler`
        private static bool IsBuildTime => Assembly.GetEntryAssembly() != null;

        public void Initialize(GeneratorInitializationContext context)
        {
            if (IsBuildTime)
                context.RegisterForSyntaxNotifications(() => new InputActionsSyntaxReceiver());
        }

// Code formatter does not like raw literals
//*begin-nonstandard-formatting*
        public void Execute(GeneratorExecutionContext context)
        {
            // only run at build time
            if (!IsBuildTime)
                return;

            // TODO(JIM): Disabling this for now as it creates a chicken and egg scenario. Types are not available if not used,
            // but then how could you begin to use them if the SourceGenerator didn't run first?
            // Also note the mention about InputActions needing to be internal - it really needs to be public.

            // only generate the InputActions class for assemblies that have references to it. This means the class
            // might end up in multiple user assemblies, which is why they're all internal.
            //if (context.SyntaxReceiver is not InputActionsSyntaxReceiver syntaxReceiver ||
            //    syntaxReceiver.InputActionsReferences.Count == 0)
            //    return;

            context.CancellationToken.ThrowIfCancellationRequested();

            var additionalFile = context.AdditionalFiles.FirstOrDefault();
            if (additionalFile == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("ISGEN001", "Additional files not specified.", "", "InputSystemSourceGenerator", DiagnosticSeverity.Error, true, "No additional files were specified."), null));
                return;
            }

            var projectPath = additionalFile.GetText().ToString();
            var globalActionsAssetPath = Path.Combine(projectPath, GlobalActionsAssetPath);
            if (!File.Exists(globalActionsAssetPath))
            {
                Console.Error.Write($"Global input actions asset file not found at '{globalActionsAssetPath}'.");
                return;
            }

            using var fileStream = new FileStream(globalActionsAssetPath, FileMode.Open);
            InputActionAsset inputActionAssetNullable = default;
            try
            {
                inputActionAssetNullable = JsonSerializer.Deserialize<InputActionAsset>(
                    fileStream,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        IncludeFields = true,
                        Converters ={
                            new JsonStringEnumConverter()
                        }
                    });
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("ISGEN002", "", $"Couldn't parse global input actions asset.", "InputSystemSourceGenerator",
                        DiagnosticSeverity.Error, true), null));
                return;
            }

            var source =
"""
// <auto-generated/>
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HighLevel;
using Input=UnityEngine.InputSystem.HighLevel.Input;


    public static partial class InputActions
    {

""";

            var inputActionAsset = inputActionAssetNullable;
            foreach (var actionMap in inputActionAsset.Maps)
            {
                source +=
                    $$"""
                    public class {{GenerateInputActionMapClassName(actionMap)}}
                    {
                        public {{GenerateInputActionMapClassName(actionMap)}}()
                        {
                            {{actionMap.Actions.Render(a =>
                                $"{FormatFieldName(a.Name)} = new {GetInputActionWrapperType(a)}(Input.globalActions.FindAction(\"{actionMap.Name}/{a.Name}\"));{Environment.NewLine}")}}
                        }

                        {{GenerateInputActionProperties(actionMap)}}
                    }

                    public static {{GenerateInputActionMapClassName(actionMap)}} {{FormatFieldName(actionMap.Name)}};{{Environment.NewLine}}
                    """;
            }

            source +=
                $$"""
                static InputActions()
                {
                    {{GenerateInstantiateInputActionMaps(inputActionAsset.Maps)}}
                }
                """;
            source += " }";

            context.AddSource($"InputSystemGlobalActionsSourceGenerator_{GetStableHashCode(source)}.g.cs", SourceText.From(source, Encoding.UTF8));

            File.WriteAllText(Path.Combine(projectPath, "temp\\InputSystemGlobalActionsSourceGenerator.g.cs"), source);
        }

        private string GenerateInputActionWrapper(InputAction inputAction)
        {
            var interactionFields = "";
            var ctorBody = "";
            return $$"""
                public class {{FormatClassName(inputAction.Name)}}Input
                {
                    {{interactionFields}}

                    internal {{FormatClassName(inputAction.Name)}}Input()
                    {

                    }
                }
                """;
        }

        private string GetInputActionWrapperType(InputAction inputAction)
        {
            if (string.IsNullOrEmpty(inputAction.Interactions))
                return $"Input<{GetTypeFromExpectedType(inputAction.ExpectedControlType)}>";

            return $"{inputAction.Name}Input";
        }

        private string GenerateInputActionProperties(ActionMap actionMap)
        {
            var source = string.Empty;
            foreach (var action in actionMap.Actions)
            {
                var typeFromExpectedType = GetTypeFromExpectedType(action.ExpectedControlType);

                var bindings = actionMap.Bindings.Where(b => b.Action == action.Name).ToList();

                var bindingString = string.Empty;
                for (var i = 0; i < bindings.Count; i++)
                {
                    if (bindings[i].IsComposite)
                    {
                        bindingString += $"/// {bindings[i].Name}{Environment.NewLine}";

                        i++;
                        while (i < bindings.Count && bindings[i].IsPartOfComposite)
                        {
                            bindingString += $"/// {bindings[i].Name}:{SecurityElement.Escape(bindings[i].Path)}{Environment.NewLine}";
                            i++;
                        }
                    }
                    else
                    {
                        bindingString += $"/// {SecurityElement.Escape(bindings[i].Path)}{Environment.NewLine}";
                    }
                }

                // remove the trailing newline
                if (bindings.Count > 0)
                    bindingString = bindingString.TrimEnd('\n', '\r');

                source += $$"""
                    /// <summary>
                    /// This action is currently bound to the following control paths:
                    ///
                    /// <example>
                    /// <code>
                    ///
                    {{bindingString}}
                    ///
                    /// </code>
                    /// </example>
                    /// </summary>

                    """;

                source += $"public Input<{typeFromExpectedType}> {FormatFieldName(action.Name)}  {{ get; }}{Environment.NewLine}";
            }

            return source;
        }
//*end-nonstandard-formatting*

        private string GenerateInstantiateInputActionMaps(ActionMap[] actionMaps)
        {
            var str = "";
            foreach (var actionMap in actionMaps)
            {
                str += $"{FormatFieldName(actionMap.Name)} = new {GenerateInputActionMapClassName(actionMap)}();{Environment.NewLine}";
            }

            return str;
        }

        private static string GenerateInputActionMapClassName(ActionMap actionMap)
        {
            // TODO: More robust class name generation. Replace incompatible characters
            var actionMapClassName = actionMap.Name.Replace(" ", "");

            return FormatClassName(actionMapClassName) + "InputActionMap";
        }

        private static string FormatClassName(string str)
        {
            return "_" + char.ToUpper(str[0]) + str.Substring(1);
        }

        private static string FormatFieldName(string str)
        {
            if (str.Length <= 3)
                return (char.IsDigit(str[0]) ? "_" : "") + str.ToUpper();

            return (char.IsDigit(str[0]) ? "_" : "") + char.ToLower(str[0]) + str.Substring(1);
        }

        private string GetTypeFromExpectedType(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Analog:
                case ControlType.Axis:
                case ControlType.Button:
                case ControlType.Delta:
                case ControlType.DiscreteButton:
                case ControlType.Key:
                    return nameof(Single);

                case ControlType.Digital:
                case ControlType.Integer:
                    return nameof(Int32);

                case ControlType.Double:
                    return nameof(Double);

                case ControlType.Bone:
                    return "UnityEngine.InputSystem.XR.Bone";

                case ControlType.Dpad:
                case ControlType.Vector2:
                case ControlType.Stick:
                    return "UnityEngine.Vector2";

                case ControlType.Vector3:
                    return "UnityEngine.Vector3";

                case ControlType.Eyes:
                    return "UnityEngine.InputSystem.XR.Eyes";

                case ControlType.Pose:
                    return "UnityEngine.InputSystem.XR.PoseState";

                case ControlType.Quaternion:
                    return "UnityEngine.Quaternion";

                case ControlType.Touch:
                    return "UnityEngine.InputSystem.LowLevel.TouchState";

                default:
                    return null;
            }
        }

        public static int GetStableHashCode(string str)
        {
            unchecked
            {
                var hash1 = 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }

    public struct InputActionAsset
    {
        public string Name;
        public ActionMap[] Maps;
        public ControlScheme[] ControlSchemes;
    }

    public struct ActionMap
    {
        public string Id;
        public string Name;
        public InputAction[] Actions;
        public Binding[] Bindings;
    }

    public class InputAction
    {
        public string Id;
        public ActionType Type;
        public string Name;
        public ControlType ExpectedControlType;
        public string Processors;
        public string Interactions;
    }

    public class Binding
    {
        public string Id;
        public string Name;
        public string Path;
        public string Interactions;
        public string Groups;
        public string Action;
        public bool IsComposite;
        public bool IsPartOfComposite;
    }

    public class ControlScheme
    {
    }

    public enum ActionType
    {
        Button,
        Value,
        Passthrough
    }

    public enum ControlType
    {
        Analog,
        Axis,
        Bone,
        Button,
        Delta,
        Digital,
        DiscreteButton,
        Double,
        Dpad,
        Eyes,
        Integer,
        Key,
        Pose,
        Quaternion,
        Stick,
        Touch,
        Vector2,
        Vector3
    }
}

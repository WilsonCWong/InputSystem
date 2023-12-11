#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA || PACKAGE_DOCS_GENERATION

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
    /// <summary>
    /// This is abstract input report for PS5 DualSense controller, similar to what is on the wire, but not exactly binary matching any state events.
    /// See ConvertInputReport for the exact conversion.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 9 /* !!! Beware !!! If you plan to increase this, think about how you gonna fit 10 byte state events because we can only shrink events in IEventPreProcessor */)]
    public struct DualSenseHIDInputReport : IInputStateTypeInfo
    {
        public static FourCC Format = new FourCC('D', 'S', 'V', 'S'); // DualSense Virtual State
        public FourCC format => Format;

        [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(0)] public byte leftStickX;
        [FieldOffset(1)] public byte leftStickY;

        [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(2)] public byte rightStickX;
        [FieldOffset(3)] public byte rightStickY;

        [InputControl(name = "leftTrigger", format = "BYTE")]
        [FieldOffset(4)] public byte leftTrigger;

        [InputControl(name = "rightTrigger", format = "BYTE")]
        [FieldOffset(5)] public byte rightTrigger;

        [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
        [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "buttonWest", displayName = "Square", bit = 4)]
        [InputControl(name = "buttonSouth", displayName = "Cross", bit = 5)]
        [InputControl(name = "buttonEast", displayName = "Circle", bit = 6)]
        [InputControl(name = "buttonNorth", displayName = "Triangle", bit = 7)]
        [FieldOffset(6)] public byte buttons0;

        [InputControl(name = "leftShoulder", bit = 0)]
        [InputControl(name = "rightShoulder", bit = 1)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = 2)]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = 3)]
        [InputControl(name = "select", displayName = "Share", bit = 4)]
        [InputControl(name = "start", displayName = "Options", bit = 5)]
        [InputControl(name = "leftStickPress", bit = 6)]
        [InputControl(name = "rightStickPress", bit = 7)]
        [FieldOffset(7)] public byte buttons1;

        [InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0)]
        [InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1)]
        [InputControl(name = "micButton", layout = "Button", displayName = "Mic Mute", bit = 2)]
        [FieldOffset(8)] public byte buttons2;
    }
    
        [StructLayout(LayoutKind.Explicit, Size = 47)]
    internal struct DualSenseHIDOutputReportPayload
    {
        [FieldOffset(0)] public byte enableFlags1;
        [FieldOffset(1)] public byte enableFlags2;
        [FieldOffset(2)] public byte highFrequencyMotorSpeed;
        [FieldOffset(3)] public byte lowFrequencyMotorSpeed;
        [FieldOffset(44)] public byte redColor;
        [FieldOffset(45)] public byte greenColor;
        [FieldOffset(46)] public byte blueColor;
    }

    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct DualSenseHIDUSBOutputReport : IInputDeviceCommandInfo
    {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
        public FourCC typeStatic => Type;

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 48;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte reportId;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public DualSenseHIDOutputReportPayload payload;

        public static DualSenseHIDUSBOutputReport Create(DualSenseHIDOutputReportPayload payload)
        {
            return new DualSenseHIDUSBOutputReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = 2,
                payload = payload
            };
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct DualSenseHIDBluetoothOutputReport : IInputDeviceCommandInfo
    {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
        public FourCC typeStatic => Type;

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 78;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte reportId;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public byte tag1;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)] public byte tag2;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 3)] public DualSenseHIDOutputReportPayload payload;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 74)] public uint crc32;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public unsafe fixed byte rawData[74];

        public static DualSenseHIDBluetoothOutputReport Create(DualSenseHIDOutputReportPayload payload, byte outputSequenceId)
        {
            var report = new DualSenseHIDBluetoothOutputReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = 0x31,
                tag1 = (byte)((outputSequenceId & 0xf) << 4),
                tag2 = 0x10,
                payload = payload
            };

            ////FIXME: Calculate crc32 correctly
            return report;
        }
    }
}

namespace UnityEngine.InputSystem.DualShock
{
    /// <summary>
    /// PS5 DualSense controller that is interfaced to a HID backend.
    /// </summary>
    [InputControlLayout(stateType = typeof(DualSenseHIDInputReport), displayName = "DualSense HID")]
    public class DualSenseGamepadHID : DualShockGamepad, IEventMerger, IEventPreProcessor, IInputStateCallbackReceiver
    {
        // Gamepad might send 3 types of input reports:
        // - Minimal report, first byte is 0x01, observed size is 78, also can be 10
        // - Full USB report, first byte is 0x01, observed size is 64
        // - Full Bluetooth report, first byte is 0x31, observed size 78
        // While USB and Bluetooth reports only differ in header,
        // Minimal report also differs in order of fields (buttons -> triggers vs triggers -> buttons).

        public ButtonControl leftTriggerButton { get; protected set; }
        public ButtonControl rightTriggerButton { get; protected set; }
        public ButtonControl playStationButton { get; protected set; }

        public virtual Color lightBarColor
        {
            get
            {
                m_LightBarColor ??= Color.black;
                return m_LightBarColor.Value;
            }
        }

        private float? m_LowFrequencyMotorSpeed;
        private float? m_HighFrequenceyMotorSpeed;
        protected Color? m_LightBarColor;
        private byte outputSequenceId;

        protected override void FinishSetup()
        {
            leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
            rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");
            playStationButton = GetChildControl<ButtonControl>("systemButton");

            base.FinishSetup();
        }

        public override void PauseHaptics()
        {
            if (!m_LowFrequencyMotorSpeed.HasValue && !m_HighFrequenceyMotorSpeed.HasValue)
                return;

            SetMotorSpeedsAndLightBarColor(0.0f, 0.0f, m_LightBarColor);
        }

        public override void ResetHaptics()
        {
            if (!m_LowFrequencyMotorSpeed.HasValue && !m_HighFrequenceyMotorSpeed.HasValue)
                return;

            m_HighFrequenceyMotorSpeed = null;
            m_LowFrequencyMotorSpeed = null;

            SetMotorSpeedsAndLightBarColor(m_LowFrequencyMotorSpeed, m_HighFrequenceyMotorSpeed, m_LightBarColor);
        }

        public override void ResumeHaptics()
        {
            if (!m_LowFrequencyMotorSpeed.HasValue && !m_HighFrequenceyMotorSpeed.HasValue)
                return;
            SetMotorSpeedsAndLightBarColor(m_LowFrequencyMotorSpeed, m_HighFrequenceyMotorSpeed, m_LightBarColor);
        }

        public override void SetLightBarColor(Color color)
        {
            m_LightBarColor = color;
            SetMotorSpeedsAndLightBarColor(m_LowFrequencyMotorSpeed, m_HighFrequenceyMotorSpeed, m_LightBarColor);
        }

        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            m_LowFrequencyMotorSpeed = lowFrequency;
            m_HighFrequenceyMotorSpeed = highFrequency;
            SetMotorSpeedsAndLightBarColor(m_LowFrequencyMotorSpeed, m_HighFrequenceyMotorSpeed, m_LightBarColor);
        }

        /// <summary>
        /// Set motor speeds of both motors and the light bar color simultaneously.
        /// </summary>
        /// <param name="lowFrequency"><see cref="IDualMotorRumble.SetMotorSpeeds"/></param>
        /// <param name="highFrequency"><see cref="IDualMotorRumble.SetMotorSpeeds"/></param>
        /// <param name="color"><see cref="IDualShockHaptics.SetLightBarColor"/></param>
        /// <returns>True if the command succeeded. Will return false if another command is currently being processed.</returns>
        /// <remarks>
        /// Use this method to set both the motor speeds and the light bar color in the same call. This method exists
        /// because it is currently not possible to process an input/output control (IOCTL) command while another one
        /// is in flight. For example, calling <see cref="SetMotorSpeeds"/> immediately after calling
        /// <see cref="SetLightBarColor"/> might result in only the light bar color changing. The <see cref="SetMotorSpeeds"/>
        /// call could fail. It is however possible to combine multiple IOCTL instructions into a single command, which
        /// is what this method does.
        ///
        /// See <see cref="IDualMotorRumble.SetMotorSpeeds"/> and <see cref="IDualShockHaptics.SetLightBarColor"/>
        /// for the respective documentation regarding setting rumble and light bar color.</remarks>
        public bool SetMotorSpeedsAndLightBarColor(float? lowFrequency, float? highFrequency, Color? color)
        {
            var lf = lowFrequency.HasValue ? lowFrequency.Value : 0;
            var hf = highFrequency.HasValue ? highFrequency.Value : 0;
            var c = color.HasValue ? color.Value : Color.black;

            // DualSense differs a bit from DualShock 4 because all effects need to be set at a same time,
            // otherwise setting just a color would disable motor rumble.
            var payload = new DualSenseHIDOutputReportPayload
            {
                enableFlags1 = 0x1 | // Enable motor rumble.
                    0x2, // Disable haptics.
                enableFlags2 = 0x4, // Enable LEDs color.
                lowFrequencyMotorSpeed = (byte)NumberHelpers.NormalizedFloatToUInt(lf, byte.MinValue, byte.MaxValue),
                highFrequencyMotorSpeed = (byte)NumberHelpers.NormalizedFloatToUInt(hf, byte.MinValue, byte.MaxValue),
                redColor = (byte)NumberHelpers.NormalizedFloatToUInt(c.r, byte.MinValue, byte.MaxValue),
                greenColor = (byte)NumberHelpers.NormalizedFloatToUInt(c.g, byte.MinValue, byte.MaxValue),
                blueColor = (byte)NumberHelpers.NormalizedFloatToUInt(c.b, byte.MinValue, byte.MaxValue)
            };

            ////FIXME: Bluetooth reports are not working
            //var command = DualSenseHIDBluetoothOutputReport.Create(payload, ++outputSequenceId);
            var command = DualSenseHIDUSBOutputReport.Create(payload);
            return ExecuteCommand(ref command) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool MergeForward(DualSenseHIDUSBInputReport* currentState, DualSenseHIDUSBInputReport* nextState)
        {
            return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 &&
                currentState->buttons2 == nextState->buttons2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool MergeForward(DualSenseHIDBluetoothInputReport* currentState, DualSenseHIDBluetoothInputReport* nextState)
        {
            return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 &&
                currentState->buttons2 == nextState->buttons2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool MergeForward(DualSenseHIDMinimalInputReport* currentState, DualSenseHIDMinimalInputReport* nextState)
        {
            return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 &&
                currentState->buttons2 == nextState->buttons2;
        }

        unsafe bool IEventMerger.MergeForward(InputEventPtr currentEventPtr, InputEventPtr nextEventPtr)
        {
            if (currentEventPtr.type != StateEvent.Type || nextEventPtr.type != StateEvent.Type)
                return false;

            var currentEvent = StateEvent.FromUnchecked(currentEventPtr);
            var nextEvent = StateEvent.FromUnchecked(nextEventPtr);

            if (currentEvent->stateFormat != DualSenseHIDGenericInputReport.Format || nextEvent->stateFormat != DualSenseHIDGenericInputReport.Format)
                return false;

            if (currentEvent->stateSizeInBytes != nextEvent->stateSizeInBytes)
                return false;

            var currentGenericReport = (DualSenseHIDGenericInputReport*)currentEvent->state;
            var nextGenericReport = (DualSenseHIDGenericInputReport*)nextEvent->state;

            if (currentGenericReport->reportId != nextGenericReport->reportId)
                return false;

            if (currentGenericReport->reportId == DualSenseHIDUSBInputReport.ExpectedReportId)
            {
                if (currentEvent->stateSizeInBytes == DualSenseHIDMinimalInputReport.ExpectedSize1 ||
                    currentEvent->stateSizeInBytes == DualSenseHIDMinimalInputReport.ExpectedSize2)
                {
                    var currentState = (DualSenseHIDMinimalInputReport*)currentEvent->state;
                    var nextState = (DualSenseHIDMinimalInputReport*)nextEvent->state;
                    return MergeForward(currentState, nextState);
                }
                else
                {
                    var currentState = (DualSenseHIDUSBInputReport*)currentEvent->state;
                    var nextState = (DualSenseHIDUSBInputReport*)nextEvent->state;
                    return MergeForward(currentState, nextState);
                }
            }
            else if (currentGenericReport->reportId == DualSenseHIDBluetoothInputReport.ExpectedReportId)
            {
                var currentState = (DualSenseHIDBluetoothInputReport*)currentEvent->state;
                var nextState = (DualSenseHIDBluetoothInputReport*)nextEvent->state;
                return MergeForward(currentState, nextState);
            }
            else
                return false;
        }

        unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr eventPtr)
        {
            if (eventPtr.type != StateEvent.Type)
                return eventPtr.type != DeltaStateEvent.Type; // only skip delta state events

            var stateEvent = StateEvent.FromUnchecked(eventPtr);
            if (stateEvent->stateFormat == DualSenseHIDInputReport.Format)
                return true; // if someone queued DSVS directly, just use as-is

            var size = stateEvent->stateSizeInBytes;
            if (stateEvent->stateFormat != DualSenseHIDGenericInputReport.Format || size < sizeof(DualSenseHIDInputReport))
                return false; // skip unrecognized state events otherwise they will corrupt control states

            var genericReport = (DualSenseHIDGenericInputReport*)stateEvent->state;
            if (genericReport->reportId == DualSenseHIDUSBInputReport.ExpectedReportId)
            {
                if (stateEvent->stateSizeInBytes == DualSenseHIDMinimalInputReport.ExpectedSize1 ||
                    stateEvent->stateSizeInBytes == DualSenseHIDMinimalInputReport.ExpectedSize2)
                {
                    // minimal report
                    var data = ((DualSenseHIDMinimalInputReport*)stateEvent->state)->ToHIDInputReport();
                    *((DualSenseHIDInputReport*)stateEvent->state) = data;
                }
                else
                {
                    var data = ((DualSenseHIDUSBInputReport*)stateEvent->state)->ToHIDInputReport();
                    *((DualSenseHIDInputReport*)stateEvent->state) = data;
                }

                stateEvent->stateFormat = DualSenseHIDInputReport.Format;
                return true;
            }
            else if (genericReport->reportId == DualSenseHIDBluetoothInputReport.ExpectedReportId)
            {
                var data = ((DualSenseHIDBluetoothInputReport*)stateEvent->state)->ToHIDInputReport();
                *((DualSenseHIDInputReport*)stateEvent->state) = data;
                stateEvent->stateFormat = DualSenseHIDInputReport.Format;
                return true;
            }
            else
                return false; // skip unrecognized reportId
        }

        public void OnNextUpdate() { }

        // filter out three lower bits as jitter noise
        internal const byte JitterMaskLow = 0b01111000;
        internal const byte JitterMaskHigh = 0b10000111;

        public unsafe void OnStateEvent(InputEventPtr eventPtr)
        {
            if (eventPtr.type == StateEvent.Type && eventPtr.stateFormat == DualSenseHIDInputReport.Format)
            {
                var currentState = (DualSenseHIDInputReport*)((byte*)currentStatePtr + m_StateBlock.byteOffset);
                var newState = (DualSenseHIDInputReport*)StateEvent.FromUnchecked(eventPtr)->state;

                var actuated =

                    // we need to make device current if axes are outside of deadzone specifying hardware jitter of sticks around zero point
                    newState->leftStickX < JitterMaskLow
                    || newState->leftStickX > JitterMaskHigh
                    || newState->leftStickY < JitterMaskLow
                    || newState->leftStickY > JitterMaskHigh
                    || newState->rightStickX < JitterMaskLow
                    || newState->rightStickX > JitterMaskHigh
                    || newState->rightStickY < JitterMaskLow
                    || newState->rightStickY > JitterMaskHigh

                    // we need to make device current if triggers or buttons state change
                    || newState->leftTrigger != currentState->leftTrigger
                    || newState->rightTrigger != currentState->rightTrigger
                    || newState->buttons0 != currentState->buttons0
                    || newState->buttons1 != currentState->buttons1
                    || newState->buttons2 != currentState->buttons2;

                if (!actuated)
                    InputSystem.s_Manager.DontMakeCurrentlyUpdatingDeviceCurrent();
            }

            InputState.Change(this, eventPtr);
        }

        public bool GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
        {
            return false;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DualSenseHIDGenericInputReport
        {
            public static FourCC Format => new FourCC('H', 'I', 'D');

            [FieldOffset(0)]
            public byte reportId;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DualSenseHIDUSBInputReport
        {
            public const int ExpectedReportId = 0x01;

            [FieldOffset(0)]
            public byte reportId;
            [FieldOffset(1)]
            public byte leftStickX;
            [FieldOffset(2)]
            public byte leftStickY;
            [FieldOffset(3)]
            public byte rightStickX;
            [FieldOffset(4)]
            public byte rightStickY;
            [FieldOffset(5)]
            public byte leftTrigger;
            [FieldOffset(6)]
            public byte rightTrigger;
            [FieldOffset(8)]
            public byte buttons0;
            [FieldOffset(9)]
            public byte buttons1;
            [FieldOffset(10)]
            public byte buttons2;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DualSenseHIDInputReport ToHIDInputReport()
            {
                return new DualSenseHIDInputReport
                {
                    leftStickX = leftStickX,
                    leftStickY = leftStickY,
                    rightStickX = rightStickX,
                    rightStickY = rightStickY,
                    leftTrigger = leftTrigger,
                    rightTrigger = rightTrigger,
                    buttons0 = buttons0,
                    buttons1 = buttons1,
                    buttons2 = (byte)(buttons2 & 0x07)
                };
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DualSenseHIDBluetoothInputReport
        {
            public const int ExpectedReportId = 0x31;

            [FieldOffset(0)]
            public byte reportId;
            [FieldOffset(2)]
            public byte leftStickX;
            [FieldOffset(3)]
            public byte leftStickY;
            [FieldOffset(4)]
            public byte rightStickX;
            [FieldOffset(5)]
            public byte rightStickY;
            [FieldOffset(6)]
            public byte leftTrigger;
            [FieldOffset(7)]
            public byte rightTrigger;
            [FieldOffset(9)]
            public byte buttons0;
            [FieldOffset(10)]
            public byte buttons1;
            [FieldOffset(11)]
            public byte buttons2;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DualSenseHIDInputReport ToHIDInputReport()
            {
                return new DualSenseHIDInputReport
                {
                    leftStickX = leftStickX,
                    leftStickY = leftStickY,
                    rightStickX = rightStickX,
                    rightStickY = rightStickY,
                    leftTrigger = leftTrigger,
                    rightTrigger = rightTrigger,
                    buttons0 = buttons0,
                    buttons1 = buttons1,
                    buttons2 = (byte)(buttons2 & 0x07)
                };
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DualSenseHIDMinimalInputReport
        {
            public static int ExpectedSize1 = 10;
            public static int ExpectedSize2 = 78;

            [FieldOffset(0)]
            public byte reportId;
            [FieldOffset(1)]
            public byte leftStickX;
            [FieldOffset(2)]
            public byte leftStickY;
            [FieldOffset(3)]
            public byte rightStickX;
            [FieldOffset(4)]
            public byte rightStickY;
            [FieldOffset(5)]
            public byte buttons0;
            [FieldOffset(6)]
            public byte buttons1;
            [FieldOffset(7)]
            public byte buttons2;
            [FieldOffset(8)]
            public byte leftTrigger;
            [FieldOffset(9)]
            public byte rightTrigger;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DualSenseHIDInputReport ToHIDInputReport()
            {
                return new DualSenseHIDInputReport
                {
                    leftStickX = leftStickX,
                    leftStickY = leftStickY,
                    rightStickX = rightStickX,
                    rightStickY = rightStickY,
                    leftTrigger = leftTrigger,
                    rightTrigger = rightTrigger,
                    buttons0 = buttons0,
                    buttons1 = buttons1,
                    buttons2 = (byte)(buttons2 & 0x03) // higher bits seem to contain random data, and mic button is not supported
                };
            }
        }
    }
}

#endif // UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA
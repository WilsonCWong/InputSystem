using System;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;

internal class DualSenseTests : CoreTestsFixture
{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA || UNITY_EDITOR
    [Test]
    [Category("Devices")]
    [TestCase(0x54C, 0xCE6)]
    public void Devices_SupportsDualsenseAsHID_WithJustPIDAndVID(int vendorId, int productId)
    {
        var device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "HID",
            capabilities = new HID.HIDDeviceDescriptor
            {
                vendorId = vendorId,
                productId = productId,
            }.ToJson()
        });

        Assert.That(device, Is.AssignableTo<DualSenseGamepadHID>());
    }
#endif
}

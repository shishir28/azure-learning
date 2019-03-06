using System;

namespace Monad.Eventhubs.Core
{
    public class DeviceTelemetry
    {
        public string IpAddress { get; set; }
        public DateTime Time { get; set; }

        public DeviceType DeviceType { get; set; }

        public bool IsOn { get; set; }

        public override string ToString()
        {
            return $"{DeviceType}:{IpAddress}";
        }

        public static DeviceTelemetry GenerateRandom(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            return new DeviceTelemetry
            {
                IpAddress = GenerateRandomIpAddress(random),
                DeviceType = GenerateRandomDevice(random),
                Time = DateTime.UtcNow,
                IsOn = random.Next(0, 2).Equals(1)
            };
        }

        private static string GenerateRandomIpAddress(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            return $"{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";
        }

        private static DeviceType GenerateRandomDevice(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            var values = Enum.GetValues(typeof(DeviceType));
            return (DeviceType)values.GetValue(random.Next(1, values.Length));
        }

    }
}

using CodeMechanic.FileSystem;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;
using Shargs;

public class PushbulletService
{
    private readonly ArgsMap arguments;
    private readonly string pat;

    public PushbulletService(ArgsMap arguments)
    {
        this.arguments = arguments;
        bool debug = this.arguments.HasFlag("--debug");
        this.pat = DotEnv.Get("PUSHBULLET_PAT") ?? string.Empty;

        if (debug)
            Console.WriteLine($"Pat: {pat}");
    }

    public async Task Send(string title, string message = @"""hello world""")
    {
        PushbulletClient client = new PushbulletClient(pat);

        //If you don't know your device_iden, you can always query your devices
        var devices = client.CurrentUsersDevices();
        // if(debug) devices.Dump("my devices");

        bool send_notification = true;
        var device = devices.Devices.FirstOrDefault(o => o.Manufacturer == "samsung");

        if (device != null && send_notification)
        {
            PushNoteRequest request = new PushNoteRequest
            {
                DeviceIden = device.Iden,
                Title = title,
                Body = message,
                // <span style='{background: red; text-color: white;}' C# wrapper />
            };

            PushResponse response = client.PushNote(request);
        }
    }
}

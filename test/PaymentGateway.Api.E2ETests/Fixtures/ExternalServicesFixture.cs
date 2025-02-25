using System.Diagnostics;

namespace PaymentGateway.Api.E2ETests.Fixtures;

public class ExternalServicesFixture : IAsyncLifetime
{
    private const string DockerComposeFilePath = "./";

    public async Task InitializeAsync()
    {
        await RunDockerComposeCommandAsync("up -d");
        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    public async Task DisposeAsync()
    {
        await RunDockerComposeCommandAsync("down");
    }

    private static async Task RunDockerComposeCommandAsync(string arguments)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "docker-compose",
            Arguments = arguments,
            WorkingDirectory = DockerComposeFilePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using Process? process = Process.Start(startInfo);
        if (process != null)
        {
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                throw new Exception($"docker-compose {arguments} failed: {error}");
            }
        }
    }
}
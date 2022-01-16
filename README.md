# neocluster
Application to get telemetry data from Forza Horizon 5. For now, it only grabs speed, RPM and gear data.

![Screenshot](Docs/screenshot.png)

## Building

Open NeoCluster.sln with Visual Studio 2022 and build. Setup Forza 5 to send UDP packets to the computer running this applications, on port 5678. Open the application and click "Start/Stop" to begin receiving data. The "Settings" button turns fullscreen on/off.

## References

[Forza Motorsport Data Out Reference](https://forums.forzamotorsport.net/turn10_postst128499_Forza-Motorsport-7--Data-Out--feature-details.aspx)
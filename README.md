### WeatherSensorsMockService
A sample project to experience with Google Protocol Buffers (protoBuf) and GRPC.
The solution contains 3 projects:
* Weather.Data - data about sensor and samples. Provides an opportunity to aggregate samples;
* Weather.Client - get responses from Weather.Emulator. Provides to user a controller to subscribe for sensors update, get information about sensors information;
* Weather.Emulator - emulates weather sensors behaviour and sends samples information to subsribed clients.
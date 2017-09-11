# AMIMonitor

## What is it?
This software will be used for checking status of AMI Proxy Server and notifying to Administrator.

## How to Configure (`AMI_Monitor.exe.conf`)

 - Set up token in order to send line notification.
   - Format:
   ```xml
   <add key="token" value="value-of-token-generated-by-line"></add>
   ```
   - Example
   ```xml
   <add key="token" value="TRp6byyCsJG7S2poh5ON3zdH88SSm3LMffZ1fXy8o1H"></add>
   ```
 - Add AMI Proxy Server in order to monitor.
   - Format:
     **DurationTime** is the time waiting for sending next request.
   ```xml
   <add key="Service_name" value="IP:PORT,DurationTime" />
   ```
   - Example:
   ```xml
   <add key="service1" value="192.168.243.137:59000,30" />
   <add key="service2" value="192.168.243.137:59001,60" />
   ```
## Example of Configuration
   ```xml
   <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
      </configSections>
      <appSettings>
        <!-- for production : TRp6byyCsJG7S2poh5ON3zdH88SSm3LMffZ1fXy8o1H -->
        <!-- for Test : 5MIkfmCenOQ57YoCCq5F2pg0DycCfLjP5B3IdrUbxKs-->
        <add key="token" value="5MIkfmCenOQ57YoCCq5F2pg0DycCfLjP5B3IdrUbxKs"></add>
        <add key="service1" value="192.168.243.137:59000,30" />
        <add key="service2" value="192.168.243.137:59001,60" />
        <add key="service3" value="192.168.243.137:59002,120" />
      </appSettings>
    </configuration>
   ```
   
## Command Line
  Used for control the each of services.
```text
=============================================================================
================================:: Command ::================================
=============================================================================
| start <Service> <IP:PORT,duration> | Use for starting new service.        |
| stop <Service>                     | Use for stopping the running service.|
| status                             | Display all of the running services. |
| exit                               | Close all services and exit program. |
=============================================================================
```
  

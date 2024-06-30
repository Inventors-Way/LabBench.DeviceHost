# LabBench DeviceHost, Beta
The LabBench DeviceHost provides a socket based interface that allow LabBench devices such as the LabBench CPAR+ device to be used from 3rd party software such as Presention(R) from [Neuro Behavioral Systems](https://www.neurobs.com/menu_presentation/menu_features/features_overview), E-Prime from [Psychology Software Tools](https://pstnet.com/products/e-prime/) or [PsychoPy](https://www.psychopy.org/).

LabBench devices is controlled by a binary serial communication protocol [Embedded Communication Protocol (ECP)](https://github.com/Inventors-Way/Inventors.ECP). This protocol is open with an open source implementation. However, as it is a relatively complex binary protocol it is challenging to implement it directly in software such as Presentation(R) or E-Prime. To alleviate this the LabBench DeviceHost acts as a bridge between the Embedded Communication Protocol (ECP) and 3rd party software. 

Using the LabBench Device Host consists of sending text commands in the form of:

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD PING;
END;
```

To a socket whos IP address and port number if configured when starting the LabBench Device Host. If a CPAR+ device is found on COM8 the Device Host will respond with:

```
OK;CPAR+ DUE, Rev. 1.0.1;
```

In general commands will respond with either ```OK;```, ```ERR;[Error Code]```, or ```[Data from the device]```. For example, if no device was connected to COM8 the command will instead respond with:

```
ERR;DeviceClosed;
```

For a full list of possible error codes, please see section Error Codes

## Installation

The LabBench Device Host is a single self-contained executable file (devhost.exe) that can be downloaded from the [Releases](https://github.com/Inventors-Way/LabBench.DeviceHost/releases) of this repository. Download it to a folder on your computer and place this folder on the path.

One complication is that as it is an executable file your computer might mark this file as potentially unsafe and not allow you to download it. How to solve it is illustrated for Explorer, however, all browsers have similar mechanisms.

For Explorer this will look like:

![Smartscreen Warnig](SmartscreenWarning01.png)

which will result in the following dialog:

![Smartscreen Warnig](SmartscreenWarning02.png)

In which you must choose Keep Anyway in order to successfully download the LabBench Device Host. In the future, once the LabBench Device Host is out of Beta, we plan to create a signed installer that bypasses this problem.

## Starting the device host

The device host can be started with its default configuration by calling the ```devhost``` command without any parameters:

![Starting the Device Host](StartingDevHost.png)

The text displayed by the Device Host is its log output. If your 3rd party software does not work as intended it can be helpful to check this log for error messages as these are more detailed than the error codes returned by the Device Host over the socket interface.

The Device Host can be configured with the following command line parameters:

|Parameter          | Description|
|-------------------|-----------|
|```-k --api-key``` | The API Key for the device host (default: 1234) |
|```-a --address``` | The address on which to host the device host (default: 127.0.0.1 / localhost) |
|```-p --port```    | The port on which to bind the device host (default: 9797) |
|```-l --log-file```| Log File to which to write the log, if left out the log will only be written to console. |

Help for these can also be shown by calling the devhost with a ```--help``` parameter.


## Using the DeviceHost from 3rd party software

### General format for commands

The general format for all commands is shown below:

```
START [API-KEY];
USE [SYSTEM] {PORT} {DEVICE};
CMD [COMMAND];
{COMMAND STATEMENTS}
END;
```

This format consists of a list of statements each terminated by a semicoloon ```;```. The ```[]``` notation means a part that is mandatory and must be specified for all commands, and the ```{}``` specifies a part that is only mandatory for certain types of systems or commands.

#### START Statement

All commands must start with a START Statement of the form:

```
START [API-KEY];
```

Where the ```[API-KEY]``` is configured when the device host is envoked. Commands send to the Device Host with a wrong API key will not be executed. However, for security purposes the Device Host will in the case of a wrong API key pretent it was successfull and respond with a ```OK;``` response. 

Please monitor the log, where an error will be displayed, if you suspect that this is the case for your 3rd party software.

#### USE Statement

Following the START Statement all commands must be followed by a USE statement in the following format:

```
USE [SYSTEM] {PORT} {DEVICE};
```

The ```SYSTEM``` parameter specifies which system to use. Possible values:

1. **SERVER**: Internal functionality for listing ports, creating and deleting device handlers.
2. **PORT**: Functionality to communicate with LabBench devices.

The ```PORT``` parameter is required for the PORT system, and specifies which port to access (i.e. COM4).

The ```DEVICE``` parameter specifies which device the host should expect to find on the port. Possible values:

1. **CPARPLUS**: The LabBench CPAR+ device.

#### CMD Statement

```
CMD [COMMAND];
{COMMAND STATEMENTS}
```

#### END Statement

All commands must end with an END statement in the following format:

```
END;
```

## Server Commands

#### PORTS

```
START 1234;
USE SERVER;
CMD PORTS;
END;
```

#### CREATE

```
START 1234;
USE SERVER;
CMD CREATE;
PORT COM8; 
DEVICE CPARPLUS;
END;
```

#### DELETE

```
START 1234;
USE SERVER;
CMD DELETE;
PORT COM8;
END;
```

## Devices 

### LabBench CPAR+ Device Commands

#### OPEN

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD OPEN;
END;
```

#### CLOSE

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD CLOSE;
END;
```

#### PING

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD PING;
END;
```

#### WAVEFORM

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD WAVEFORM;
CHANNEL 0;
REPEAT 2;
INSTRUCTIONS 2;
STEP 500 1000;
STEP 0 1000;
END;
```

#### START

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD START;
STOPCRITERION 0;
EXTERNALTRIGGER 0;
OVERRIDERATING 1;
OUTLET01 1;
OUTLET02 1;
END;
```

#### STOP

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD STOP;
END;
```

#### STATE

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD STATE;
END;
```

#### SIGNALS

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD SIGNALS;
END;
```

#### RATING

```
START 1234;
USE PORT COM8 CPARPLUS;
CMD RATING;
END;
```


## Error Codes

|Code|Description|
|----|-----------|
|NoHandlerFound| |
|InvalidCommandFormat| |
|InvalidStartOfCommand| |
|MissingUseStatement| |
|InvalidEndOfCommand| |
|NoCommandStatement| |
|NoApiKey| |
|InvalidNumberOfInstructions| |
|InvalidStepInstruction| |
|InvalidIncrementInstruction| |
|InvalidDecrementInstruction| |
|UnknownInstruction| |
|InvalidStartCommandContent| |
|InvalidParameterSpecification| |
|InvalidInteger| |
|UnknownCommand| |
|OpenFailed| |
|NoStatus| |
|DeviceClosed| |
|CommunicationFailure| |
|IncompatibleDevice| |
|CloseFailed| |
|InvalidCommandContent| |
|NoPortStatement| |
|NoDeviceStatement| |
|HandlerExists| |
|UnknownDevice| |


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

**Please note, that currently the socket must be closed and reopened between each command sent to the DeviceHost. This is due to a [bug](https://github.com/Inventors-Way/LabBench.DeviceHost/issues/1) that will be fixed.**

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

The command statement tells the DeviceHost which command to execute by the system that was selected in the USE statement.

```
CMD [COMMAND];
{COMMAND STATEMENTS}
```

For the set of possible commands, please see the description of the SYSTEM and CPARPLUS systems.

#### END Statement

All commands must end with an END statement in the following format:

```
END;
```

### Connecting to the device host

To connect to the DeviceHost, open an http socket with UTF8 encoding to address and port configured when the DeviceHost was started. If no parameters are specified this will be ```127.0.0.1:9797```.

### Procedure for accessing a device.

To use a LabBench Device:

1. Create a handler for device on its COM port (USE SYSTEM + CMD CREATE).
2. Open the COM port (USE CPARPLUS + CMD OPEN).
3. Start sending commands (USE CPARPLUS + selected commands)
4. Close the COM port (USE CPARPLUS + CMD CLOSE)
5. Delete the handler (USE SYSTEM + CMD DELETE)

If the DeviceHost is closed from the command line, then step 4 + 5 is optional, and will be executed automatically when the program is closed. To close the program from the command line, press CTRL + C.

## Server Commands

#### PORTS

List the COM ports:

```
START [API-KEY]; 
USE SERVER;
CMD PORTS;
END;
```

**Response:** This command will return a semicollon seperated list of COM ports in the form of:

```
COM1;COM2
```

#### CREATE

Create a device handler on a given ```[PORT]``` port and ```[DEVICETYPE]``` device type:

```
START 1234;
USE SERVER;
CMD CREATE;
PORT [PORT]; 
DEVICE [DEVICETYPE];
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

Parameters:
```[PORT]```: Name of the COM port. Valid values can be obtained with the SYSTEM + PORT command
```[DEVICETYPE]```: Possible values CPARPLUS

#### DELETE

Delete a device handler on a given ```[PORT]``` port:

```
START 1234;
USE SERVER;
CMD DELETE;
PORT [PORT];
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

## Devices 

### LabBench CPAR+ Device Commands

#### OPEN

Open the COM port to allow commands to be send to a device:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD OPEN;
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

#### CLOSE

Close the COM port:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD CLOSE;
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

#### PING

Will ping a device and check that it has the correct type as expected by the created handler:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD PING;
END;
```

Please, note if the port is not openend it will result in an error.

**Response::** **Response:** ```OK;[DEVICETYPE]``` if successfull, otherwise ```ERR;[ERRORCODE]```.

#### WAVEFORM

The CPARPLUS device can have up to two waveform channels that each can be routed to one or both of the two pressure outlets of the device. These programs consists of a series of instructions STEP, INC, and DEC that changes the current output pressure. To prevent errors such as infinite loops there are no brancing instructions. However, it is possible to repeat a program set number of times.

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD WAVEFORM;
CHANNEL [CHANNEL];
REPEAT [REPEAT];
INSTRUCTIONS [INSTRUCTIONS];
STEP [PRESSURE] [DURATION];
INC [DELTAPRESSURE] [DURATION];
DEC [DELTAPRESSURE] [DURATION];
END;
```

**Parameters**:

* ```[CHANNEL]```: Channel for which to set the program (valid values: 0 for Channel 1 or 1: for Channel 2)
* ```[REPEAT]```: Number of times to repeat the program
* ```[INSTRUCTIONS]```: Number of subsequent instructions, must be equal to the number of instructions after the INSTRUCTIONS statement.
* ```[PRESSURE]```: Pressure to set for the STEP command, is the pressure in kPa multiplied by 10. E.g. 500 is equal to 50kPa.
* ```[DURATION]```: Duration of the instruction in ms.
* ```[DELTAPRESSURE]```: Change in pressure per second multiplied by 10 for the INC and DEC instructions. Must be a positive value. The change in pressure is relative to the current pressure CURRENTPRESSURE, and the final pressure will be ```[DELTAPRESSURE]*[DURATION] + CURRENTPRESSURE``` for the INC instruction and ```-[DELTAPRESSURE]*[DURATION] + CURRENTPRESSURE``` for the DEC instruction.

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

#### CLEAR

Will clear all waveform programs:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD CLEAR;
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.


#### START

Will start a pressure stimulation with the currently set pressure WAVEFORM programs:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD START;
STOPCRITERION [STOPCRITERION];
EXTERNALTRIGGER [EXTERNALTRIGGER];
OVERRIDERATING [OVERRIDERATING];
OUTLET01 [WAVEFORM];
OUTLET02 [WAVEFORM];
END;
```

**Parameters**:

* ```[STOPCRITERION]```: 0: STOP_CRITERION_ON_BUTTON_VAS the pressure stimulation will stop when VAS 10 is scored or a button is pressed, 1: STOP_CRITERION_ON_BUTTON_PRESSED the pressure stimulation will stop when a button is pressed, 2: STOP_CRITERION_ON_BUTTON_RELEASED the pressure stimulation will stop when a button is released. In all cases the stimulation will automatically stop when all pressure WAVEFORM programs have completed.
* ```[EXTERNALTRIGGER]```: If set to 0 the stimulation will start immediately, if set to 1 the stimulation will start when a trigger is received on the TRIG IN connection on the device.
* ```[OVERRIDERATING]```: If set to 0 the stimulation will not start if VAS != 0, if set to 1 the stimulation will start regardless of the current VAS rating. 
* ```[WAVEFORM]```: Which WAVEFORM program is routed to the pressure outlet. 0: None, 1: Channel 1, 2: Channel 2.

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

#### STOP

Stops any active pressure stimulation:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD STOP;
END;
```

**Response:** ```OK;``` if successfull, otherwise ```ERR;[ERRORCODE]```.

Please note, if the intention is to stop a stimulation, no harm will come from executing this command even if a pressure stimulation is not active.

#### STATE

Return the state of the device:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD STATE;
END;
```

If successfull it will return the state of the device in the form of:

```
State [STATE];
VasConnected [0 or 1];
VasIsLow [0 or 1];
PowerOn [0 or 1];
StartPossible [0 or 1];
Condition [0 or 1];
FinalPressure01 [PRESSURE]
FinalPressure02 [PRESSURE]
SupplyPressureLow [0 or 1]
SupplyPressure [PRESSURE]
```

**Values**:
 
* ```[STATE]```: State of the device. Possible values STATE_NOT_CONNECTED, STATE_IDLE, STATE_STATE_STIMULATING, STATE_EMERGENCY, STATE_PENDING. STATE_NOT_CONNECTED: COM port is not open. STATE_IDLE: No active pressure stimulation. STATE_EMERGENCY: Emergency button is active and pressure is vented to ambient air. STATE_PENDING: The device is active but waiting for an external trigger to start pressure stimulation.
* ```FinalPresureXX```: The final pressure for outlet XX when the pressure stimulation was completed either by fullfilment of the STOPCRITERION or when the pressure WAVEFORM programs completed. This is useful for determination of Pain Detection and Tolerance Thresholds.
* ```[PRESSURE]```: Pressure in kPa multiplied by 10.


or an error in the form of ```ERR;[ERRORCODE]```.

#### SIGNALS

Returns pressures and ratings since last execution of the SIGNALS command:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD SIGNALS;
END;
```

If successfull it will return the state of the device in the form of:

```
Pressure01;Pressure02;Rating;
{Pressure, Pressure, Rating}
```

Please note, if no pressures are available there will be no values after the header.

**Values**:
* ```{Pressure}```: Pressure in kPa multiplied by 10.
* ```{Rating}```: VAS rating in mm.

or an error in the form of ```ERR;[ERRORCODE]```.

#### RATING

Will return the current ratings:

```
START 1234;
USE PORT [PORT] CPARPLUS;
CMD RATING;
END;
```

If successfull it will return the state of the device in the form of:

```

```

or an error in the form of ```ERR;[ERRORCODE]```.

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


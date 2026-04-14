# Djmain Chart

These are the charts that are used on the Djmain
architecture.

### Note Count Section

Prior to any meaningful events, note counts are present
at the start of the file.

| Offset | Type  | Description                  |
|--------|-------|------------------------------|
| 0      | short | always zero                  |
| 2      | byte  | lower 4 bits = player number |
| 3      | byte  | number of notes up to 250    |

If the count is 250, then another note count structure
will follow. This is used to represent note counts
greater than 250, they are cumulative.

### Event Section

Each event is comprised of four bytes. The format of the event
data depends on the the event type.

| Offset | Type  | Description                                         |
|--------|-------|-----------------------------------------------------|
| 0      | short | offset in ticks                                     |
| 2      | byte  | lower 4 bits = event type, upper 4 bits = parameter |
| 3      | byte  | value                                               |

##### Data Table: Columns (beatmania)

This is a table of columns used in the beatmania games.

| ID  | Description     |
|-----|-----------------|
| 0x0 | 1P Key 0        |
| 0x1 | 2P Key 0        |
| 0x2 | 1P Key 1        |
| 0x3 | 2P Key 1        |
| 0x4 | 1P Key 2        |
| 0x5 | 2P Key 2        |
| 0x6 | 1P Key 3        |
| 0x7 | 2P Key 3        |
| 0x8 | 1P Key 4        |
| 0x9 | 2P Key 4        |
| 0xA | 1P Scratch      |
| 0xB | 2P Scratch      |
| 0xC | 1P Measure      |
| 0xD | 2P Measure      |
| 0xE | 1P Free Scratch |
| 0xF | 2P Free Scratch |

##### Data Table: Columns (pop'n music)

This is a list of columns used in the pop'n music games.

| ID  | Description  |
|-----|--------------|
| 0x0 | Left White   |
| 0x1 | Left Yellow  |
| 0x2 | Left Green   |
| 0x3 | Left Blue    |
| 0x4 | Center Red   |
| 0x5 | Right Blue   |
| 0x6 | Right Green  |
| 0x7 | Right Yellow |
| 0x8 | Right White  |

##### Data Table: Judgement Windows

| ID  | Description           |
|-----|-----------------------|
| 0x0 | Start of BAD window   |
| 0x1 | Start of GOOD window  |
| 0x2 | Start of GREAT window |
| 0x3 | End of GREAT window   |
| 0x4 | End of GOOD window    |
| 0x5 | End of BAD window     |

##### Note (type 0x0)

This kind of event represents a note or other game field
object.

| Offset | Type  | Description                               |
|--------|-------|-------------------------------------------|
| 0      | short | offset in ticks                           |
| 2      | byte  | lower 4 bits = 0x0, upper 4 bits = column |
| 3      | byte  | unused                                    |

##### Sound Assign (type 0x1)

This kind of event will assign a sound to a column.

| Offset | Type  | Description                               |
|--------|-------|-------------------------------------------|
| 0      | short | offset in ticks                           |
| 2      | byte  | lower 4 bits = 0x1, upper 4 bits = column |
| 3      | byte  | ID of the sound to assign                 |

##### Tempo/BPM (type 0x2)

This kind of event specifies a change in BPM of the song.
There will always be at least one of these events at offset 0.
The upper four bits of byte 2 represent increments of 256.

In beatmania CS, two of these can be consecutively present.
In this case, the second event represents fractional BPM: the
value is in 1/100ths of a BPM.

| Offset | Type  | Description                             |
|--------|-------|-----------------------------------------|
| 0      | short | offset in frames                        |
| 2      | byte  | lower 4 bits = 0x1, upper 4 bits = 256x |
| 3      | byte  | beats per minute                        |

##### End of Song (type 0x4)

This kind of event indicates the end of song data. Data after
this event should be ignored.

| Offset | Type  | Description                             |
|--------|-------|-----------------------------------------|
| 0      | short | offset in frames                        |
| 2      | byte  | lower 4 bits = 0x4, upper 4 bits unused |
| 3      | byte  | unused                                  |

##### Auto Play/BGM (type 0x5)

This kind of event represents a sound that will be played
automatically. The stereo panning can be set in the event
itself.

| Offset | Type  | Description                                        |
|--------|-------|----------------------------------------------------|
| 0      | short | offset in frames                                   |
| 2      | byte  | lower 4 bits = 0x5, upper 4 bits = panning 0x1-0xF |
| 3      | byte  | ID of the sound to play                            |

##### Judgement (type 0x6)

This kind of event specifies judgement windows, in frames.

| Offset | Type  | Description                                |
|--------|-------|--------------------------------------------|
| 0      | short | offset in frames                           |
| 2      | byte  | lower 4 bits = 0x6, upper 4 bits = window  |
| 3      | byte  | number of frames relative to note (signed) |

##### Judgement Sound (type 0x7)

Not much is known about this event type.

| Offset | Type  | Description                          |
|--------|-------|--------------------------------------|
| 0      | short | offset in frames                     |
| 2      | byte  | lower 4 bits = 0x7, upper 4 bits = ? |
| 3      | byte  | ?                                    |

##### Judgement Trigger (type 0x8)

Not much is known about this event type.

| Offset | Type  | Description                          |
|--------|-------|--------------------------------------|
| 0      | short | offset in frames                     |
| 2      | byte  | lower 4 bits = 0x8, upper 4 bits = ? |
| 3      | byte  | ?                                    |

##### Phrase Select (type 0x9)

Not much is known about this event type.

| Offset | Type  | Description                          |
|--------|-------|--------------------------------------|
| 0      | short | offset in frames                     |
| 2      | byte  | lower 4 bits = 0x9, upper 4 bits = ? |
| 3      | byte  | ?                                    |

##### End of Sequence

| Offset | Type  | Description |
|--------|-------|-------------|
| 0      | short | 0x7FFF      |
| 2      | byte  | 0x00        |
| 3      | byte  | 0x00        |

# Design doc

These things get ignored everywhere, but bear with me here.

### Special rules

- Infrastructure
  - Attrubutes that don't belong to any particular service
  - Extensions that don't belong to any particular service
  - Loggers, generic configurators, generic streams..
- IoC
  - These facilitate the use of dependency injection libraries by
    third parties.
  - You probably don't need to go in here. Changing any of this
    will very likely break things for third party consumers.

### Mark your classes with the correct attributes

Attributes should be attached to all classes that are to be exposed via IoC.

- Every exposed service class must have the `[Service]` attribute attached.
- Every model must have the `[Model]` attribute attached.

### Add an interface for every service

For the sake of testing and integration later on:

- When creating a new service, create an interface named the same. `MyService` would implement `IMyService`
  in the same folder.
- When accepting a service via the constructor, only use the interface version of the service. This makes
  it easy to mock up for automated unit testing.

### Breaking up individual units

There needs to be an obvious division between the following things for any
particular format:

- Raw Formats
  - Heuristics: Determining if the data is the raw format we think it is (Heuristic)
  - Streamers: Reading the raw format as-is from a stream (StreamReader)
  - Streamers: Writing the raw format back to a stream (StreamWriter)
  - Models: Defining the components of a raw format
    - These are explicitly not named, because they are not services
- Encryptions
  - Converters: Removing the encryption layer from a raw format (Decrypter)
  - Converters: Adding an encryption layer to a raw format (Encrypter)
- Compression Algorithms
  - Converters: Decompressing compressed data (Decompressor)
  - Converters: Compression data (Compressor)
- Translation Beween Formats
  - Converters: Transforming the raw format to an intermediate format such as `Sound`, `Chart`, etc (Decoder)
  - Converters: Transforming the intermediate format back to the raw format (Encoder)
  - Converters: Transforming the raw format to another raw format (Transformer)
- Containing Constants or Some Other Kind of Intrinsic Information
  - Providers: Having a list of encryption keys (Provider)
  - Providers: Exposing access to a configuration file (Provider)
- Manipulating Data (Pruning, Sorting, etc.)
  - Processors: Validating or sanitizing a data set (Sanitizer)
  - Processors: Sorting a data set (Sorter)
  - Processors: Any "generic" manipulation - take caution with this (Processor)

### Command-based formats

Formats such as SM and BMS are "command based", the interpretation and
creation of commands within these files should be split out from the act of
reading and writing them.


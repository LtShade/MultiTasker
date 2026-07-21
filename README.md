# MultiTasker
MultiTasker is inteded to enable the repair and refit of multiple mechs and vehicles at a time rather than one at a time; this increased capacity is gated behind defined dropship upgrades. This is accomplished by the *settings.json* laying out bonuses provided by specific upgrades. Furthermore, the rate at which repairs and refits can be made for subsequent parallel tasks is configurable as well if granularity is desired.

Heavily borrows from IceRaptor's mods because I liked the structure. Is it overkill for this thing? Probably. Do I care? No, I need the expose and practice.

## Assumptions
- The first position in the workorder queue will **always** operate at 100%; if you want to affect baseline efficiency, change MechTech skill.

## Settings
### Terminology
- Workorder - A mech/vehicle repair or refit request from the player
- Worker - the capacity to work on a workorder; i.e. three workers can work on three workorders simutaneously
- Efficiency - the rate at which a given worker can work on a workorder


### *settings.json* 
RepairBays[] - Array of objects that define a worker.
        {
            "WorkerID" : 0,
            "UpgradeID": "argo_mechBay1",
            "BaseEfficiency": 1
        }
- WorkerID, int
  - Is indexed to 0
  - Safe practice is to include a WorkerID of 0 tied to a permenant dropship upgrade despite the 0 position in the queue always active and at 100% efficiency
  - Do not skip an index, this is used to match to positions in the work queue
  - Example: if you define three WorkerIDs of 0, 1, and 2, you will see three workorders being worked in parallel
  - You can have multiple WorkerIDs with the same value, they are used on a first-come basis if the UpgradeID is found
    - Everything should work as expected provided the combination of WorkerID and UpgradeID are unique across this array and that the save only has one of the UpgradeIDs active
- UpgradeID, string
  - ID of the dropship upgrade that enables this worker
- BaseEfficiency, float
  - 0-1 float to determine % of MechTech skill to apply to this worker; i.e. 0.5 == 50%
  - Can be increassed in the BayEfficiencies array

BayEfficiencies[] - Array of objects that outline what upgrades, if any, update the efficiency of a worker.
- WorkerID, int
  - Indicates which worker should be affected
- UpgradeID, string
  - ID of the dropship upgrade that applies this change
NewEfficiency, float
  - 0-1 float to determine % of MechTech skill to apply to this worker; i.e. 0.5 == 50%
  - Replaces BaseEfficiency value


## Goals
- [x] Beg, borrow, and steal some boilerplate to remind myself what all connects together in this world
- [ ] Forsake my .js gods and embrace C#
- [x] Replace hardcoded reliance on two specific upgrades, make them configurable
- [x] Allow for any number of parallel repair tasks ("workers")
- [x] Allow for variable efficiencies for paths >= 1

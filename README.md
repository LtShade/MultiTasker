# MultiTasker
Potential replacement for RepairBays. Mostly an experiment to practice C#

Heavily borrows from UniversalDropshipSalesman because I liked the structure. Is it overkill for this thing? Probably. Do I care? No, I need the expose and practice.

**Assumptions**
- The first position in the Mech work order queue will **always** operate at 100%; if you want to affect baseline efficiency, change MechTech skill.


**Goals**
- [x] Beg, borrow, and steal some boilerplate to remind myself what all connects together in this world
- [ ] Forsake my .js gods and embrace C#
- [x] Replicate RepairBays functionality before refactoring it
- [x] Replace hardcoded reliance on two specific upgrades, make them configurable
- [x] Allow for any number of parallel repair tasks
- [x] Allow for variable efficiencies for paths >= 1
- [ ] Troubleshoot

using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.ValueObjects;

namespace BookRight.Infrastructure;

public class SeedData
{
    public void Initialize(AppDbContext db)
    {
        if (db.Clinics.Any()) return;

        // ── Clinics (3 — Vejle-området) ───────────────────────────────────────
        var clinic1 = Clinic.Create(
            new Address("Søndergade 14", 7100),
            [
                new TimeInterval(new DateTime(2026, 6, 1, 8, 0, 0), new DateTime(2026, 6, 1, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 2, 8, 0, 0), new DateTime(2026, 6, 2, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 3, 8, 0, 0), new DateTime(2026, 6, 3, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 4, 8, 0, 0), new DateTime(2026, 6, 4, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 5, 8, 0, 0), new DateTime(2026, 6, 5, 17, 0, 0)),
            ],
            6);

        var clinic2 = Clinic.Create(
            new Address("Damhaven 5", 7100),
            [
                new TimeInterval(new DateTime(2026, 6, 1, 9, 0, 0), new DateTime(2026, 6, 1, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 2, 9, 0, 0), new DateTime(2026, 6, 2, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 3, 9, 0, 0), new DateTime(2026, 6, 3, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 4, 9, 0, 0), new DateTime(2026, 6, 4, 17, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 5, 9, 0, 0), new DateTime(2026, 6, 5, 17, 0, 0)),
            ],
            5);

        var clinic3 = Clinic.Create(
            new Address("Boulevarden 42", 7100),
            [
                new TimeInterval(new DateTime(2026, 6, 1, 8, 0, 0), new DateTime(2026, 6, 1, 16, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 2, 8, 0, 0), new DateTime(2026, 6, 2, 16, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 3, 8, 0, 0), new DateTime(2026, 6, 3, 16, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 4, 8, 0, 0), new DateTime(2026, 6, 4, 16, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 5, 8, 0, 0), new DateTime(2026, 6, 5, 16, 0, 0)),
                new TimeInterval(new DateTime(2026, 6, 6, 9, 0, 0), new DateTime(2026, 6, 6, 13, 0, 0)),
            ],
            4);

        db.Clinics.AddRange(clinic1, clinic2, clinic3);
        db.SaveChanges();

        // ── Practitioners (12) ────────────────────────────────────────────────
        // Clinic 1 — Søndergade (fysioterapi + massage)
        var anna     = Practitioner.Create("Anna",     "Hansen",      "+4520111001", "anna.hansen@bookright.dk",       AuthorizationType.Physiotherapist, 10001);
        var peter    = Practitioner.Create("Peter",    "Christensen", "+4520111002", "peter.christensen@bookright.dk", AuthorizationType.Physiotherapist, 10002);
        var frederik = Practitioner.Create("Frederik", "Sørensen",    "+4520111003", "frederik.sorensen@bookright.dk", AuthorizationType.Masseur,         10003);

        anna.Clinics.Add(clinic1.Id);
        peter.Clinics.Add(clinic1.Id);
        frederik.Clinics.Add(clinic1.Id);

        // Clinic 1 + 2 — cross-clinic (massage + kostvejledning)
        var michael = Practitioner.Create("Michael", "Jensen",   "+4530222001", "michael.jensen@bookright.dk",   AuthorizationType.Masseur,    10004);
        var louise  = Practitioner.Create("Louise",  "Pedersen", "+4530222002", "louise.pedersen@bookright.dk",  AuthorizationType.Dietician,  10005);

        michael.Clinics.Add(clinic1.Id);
        michael.Clinics.Add(clinic2.Id);
        louise.Clinics.Add(clinic1.Id);
        louise.Clinics.Add(clinic3.Id);

        // Clinic 2 — Damhaven (fysioterapi + akupunktur + kostvejledning)
        var kasper = Practitioner.Create("Kasper", "Møller",  "+4540333001", "kasper.moller@bookright.dk",  AuthorizationType.Physiotherapist, 10006);
        var sara   = Practitioner.Create("Sara",   "Nielsen", "+4540333002", "sara.nielsen@bookright.dk",   AuthorizationType.Acupuncturist,   10007);
        var julie  = Practitioner.Create("Julie",  "Hansen",  "+4540333003", "julie.hansen2@bookright.dk",  AuthorizationType.Dietician,       10008);

        kasper.Clinics.Add(clinic2.Id);
        sara.Clinics.Add(clinic2.Id);
        julie.Clinics.Add(clinic2.Id);

        // Clinic 2 + 3 — cross-clinic (massage)
        var emma = Practitioner.Create("Emma", "Andersen", "+4550444001", "emma.andersen@bookright.dk", AuthorizationType.Masseur, 10009);

        emma.Clinics.Add(clinic2.Id);
        emma.Clinics.Add(clinic3.Id);

        // Clinic 3 — Boulevarden (fysioterapi + akupunktur)
        var jonas     = Practitioner.Create("Jonas",     "Nielsen",  "+4560555001", "jonas.nielsen2@bookright.dk",    AuthorizationType.Acupuncturist,   10010);
        var metteLar  = Practitioner.Create("Mette",     "Larsen",   "+4560555002", "mette.larsen@bookright.dk",      AuthorizationType.Physiotherapist, 10011);
        var christian = Practitioner.Create("Christian", "Poulsen",  "+4560555003", "christian.poulsen@bookright.dk", AuthorizationType.Physiotherapist, 10012);

        jonas.Clinics.Add(clinic3.Id);
        metteLar.Clinics.Add(clinic3.Id);
        christian.Clinics.Add(clinic3.Id);

        db.Practitioners.AddRange(anna, peter, frederik, michael, louise, kasper, sara, julie, emma, jonas, metteLar, christian);
        db.SaveChanges();

        // ── Patients (15 — Vejle-området) ─────────────────────────────────────
        var lars    = Patient.Create("Lars",    "Christensen", "+4550456789", "lars.christensen@mail.dk",
            new DateTime(1985,  3, 14), new Address("Nørregate 8", 7100),           "Knæsmerter ved jogging",          anna.Id);

        var mariaPed = Patient.Create("Maria",  "Pedersen",    "+4560567890", "maria.pedersen@mail.dk",
            new DateTime(1990,  7, 22), new Address("Horsensvej 22", 7100),          string.Empty,                      michael.Id);

        var anders  = Patient.Create("Anders",  "Møller",      "+4570678901", "anders.moller@mail.dk",
            new DateTime(1978, 11,  5), new Address("Flegborg 15", 7100),            "Rygproblemer - arbejdsrelateret", anna.Id);

        var sofieA  = Patient.Create("Sofie",   "Andersen",    "+4580789012", "sofie.andersen@mail.dk",
            new DateTime(1995,  1, 30), new Address("Enghavevej 30", 7100),          string.Empty,                      sara.Id);

        var thomas  = Patient.Create("Thomas",  "Nielsen",     "+4590890123", "thomas.nielsen@mail.dk",
            new DateTime(1982,  9, 18), new Address("Solsikkevej 12", 7080),         "Nakkestivhed",                    frederik.Id);

        var rasmus  = Patient.Create("Rasmus",  "Kjær",        "+4551122334", "rasmus.kjaer@mail.dk",
            new DateTime(1993,  6, 10), new Address("Dandyvej 44", 7100),            string.Empty,                      peter.Id);

        var ida     = Patient.Create("Ida",     "Svendsen",    "+4561233445", "ida.svendsen@mail.dk",
            new DateTime(2000,  2, 28), new Address("Rødkildevej 7", 7100),          "Skulder efter sportsskade",       peter.Id);

        var oliver  = Patient.Create("Oliver",  "Magnusson",   "+4571344556", "oliver.magnusson@mail.dk",
            new DateTime(1988,  8, 16), new Address("Vejlevej 52", 7120),            string.Empty,                      kasper.Id);

        var camilla = Patient.Create("Camilla", "Berg",        "+4581455667", "camilla.berg@mail.dk",
            new DateTime(1997,  4,  5), new Address("Klitrosen 18", 7100),           "Stress-relaterede spændinger",    kasper.Id);

        var emmaD   = Patient.Create("Emma",    "Dahl",        "+4591566778", "emma.dahl@mail.dk",
            new DateTime(2003, 12,  1), new Address("Fredericiavej 65", 7100),       string.Empty,                      julie.Id);

        var henrik  = Patient.Create("Henrik",  "Lund",        "+4552677889", "henrik.lund@mail.dk",
            new DateTime(1975,  7, 30), new Address("Vedelsgade 11", 7100),          "Kroniske rygsmerter",             jonas.Id);

        var lena    = Patient.Create("Lena",    "Holm",        "+4562788990", "lena.holm@mail.dk",
            new DateTime(1991, 10, 14), new Address("Rosenvænget 4", 7120),          string.Empty,                      jonas.Id);

        var nadia   = Patient.Create("Nadia",   "Vestergaard", "+4572899001", "nadia.vestergaard@mail.dk",
            new DateTime(1986,  3, 22), new Address("Strandvejen 3", 7100),          "Allergier og fordøjelse",         sara.Id);

        var mikkel  = Patient.Create("Mikkel",  "Johansen",    "+4582900112", "mikkel.johansen@mail.dk",
            new DateTime(1979, 11,  9), new Address("Grønlandsvej 28", 7100),        string.Empty,                      metteLar.Id);

        var metteBak = Patient.Create("Mette",  "Bak",         "+4592011223", "mette.bak@mail.dk",
            new DateTime(1966,  5, 17), new Address("Åboulevarden 9", 7100),         "Tennisalbue højre arm",           anna.Id);

        db.Patients.AddRange(lars, mariaPed, anders, sofieA, thomas, rasmus, ida, oliver, camilla, emmaD, henrik, lena, nadia, mikkel, metteBak);
        db.SaveChanges();

        // ── TreatmentTypes (5) ────────────────────────────────────────────────
        List<TreatmentPrice> pyshioPrices = [new TreatmentPrice(30, 395), new TreatmentPrice(45, 589), new TreatmentPrice(60, 745)];
        var physio = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, 1, pyshioPrices);
        
        List<TreatmentPrice> massagePrices = [new TreatmentPrice(30, 350), new TreatmentPrice(60, 699)];
        var massage = TreatmentType.Create("Sportsmassage", AuthorizationType.Masseur, 1, massagePrices);
        
        List<TreatmentPrice> acupuncturePrices = [new TreatmentPrice(45, 550)];
        var acupuncture = TreatmentType.Create("Akupunktur", AuthorizationType.Acupuncturist, 1, acupuncturePrices);
        
        List<TreatmentPrice> dietitianPrices = [new TreatmentPrice(30, 450), new TreatmentPrice(60, 799)];
        var dietitian = TreatmentType.Create("Kostvejledning", AuthorizationType.Dietician, 1, dietitianPrices);
        
        List<TreatmentPrice> groupTrainingPrices = [new TreatmentPrice(60, 150)];
        var groupTraining = TreatmentType.Create("Holdtræning", AuthorizationType.Physiotherapist, 6, groupTrainingPrices);

        db.TreatmentTypes.AddRange(physio, massage, acupuncture, dietitian, groupTraining);
        db.SaveChanges();

        // ── Appointments ──────────────────────────────────────────────────────

        // Lister pr. praktiserende læge til validering af overlap
        var annaAppts      = new List<Appointment>();
        var peterAppts     = new List<Appointment>();
        var frederikAppts  = new List<Appointment>();
        var michaelAppts   = new List<Appointment>();
        var louiseAppts    = new List<Appointment>();
        var kasperAppts    = new List<Appointment>();
        var saraAppts      = new List<Appointment>();
        var julieAppts     = new List<Appointment>();
        var emmaAppts      = new List<Appointment>();
        var jonasAppts     = new List<Appointment>();
        var metteLarAppts  = new List<Appointment>();
        var christianAppts = new List<Appointment>();

        // Lister pr. patient til validering af overlap
        var larsAppts    = new List<Appointment>();
        var mariaPAppts  = new List<Appointment>();
        var andersAppts  = new List<Appointment>();
        var sofieAAppts  = new List<Appointment>();
        var thomasAppts  = new List<Appointment>();
        var rasmusAppts  = new List<Appointment>();
        var idaAppts     = new List<Appointment>();
        var oliverAppts  = new List<Appointment>();
        var camillaAppts = new List<Appointment>();
        var emmaDAppts   = new List<Appointment>();
        var henrikAppts  = new List<Appointment>();
        var lenaAppts    = new List<Appointment>();
        var nadiaAppts   = new List<Appointment>();
        var mikkelAppts  = new List<Appointment>();
        var metteBAppts  = new List<Appointment>();

        // ── Gennemførte tidligere sessioner ──────────────────────────────────────

        var a1 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 20, 9, 0, 0), new DateTime(2026, 5, 20, 10, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        a1.Complete("God fremgang, fortsat styrkeøvelser anbefalet.");
        larsAppts.Add(a1); annaAppts.Add(a1); anna.Appointments.Add(a1.Id);

        // Ekstra gennemførte sessioner for Lars → samlet 4.470 kr → Bronze loyalitetsrabat (5 %)
        var aL1 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 4, 7, 10, 0, 0), new DateTime(2026, 4, 7, 11, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        aL1.Complete("Knæøvelser fortsat.");
        larsAppts.Add(aL1); annaAppts.Add(aL1); anna.Appointments.Add(aL1.Id);

        var aL2 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 3, 10, 10, 0, 0), new DateTime(2026, 3, 10, 11, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        aL2.Complete("Styrkeøvelser for knæ.");
        larsAppts.Add(aL2); annaAppts.Add(aL2); anna.Appointments.Add(aL2.Id);

        var aL3 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 2, 10, 10, 0, 0), new DateTime(2026, 2, 10, 11, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        aL3.Complete("Rehabilitering uge 6.");
        larsAppts.Add(aL3); annaAppts.Add(aL3); anna.Appointments.Add(aL3.Id);

        var aL4 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 1, 12, 10, 0, 0), new DateTime(2026, 1, 12, 11, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        aL4.Complete("Stabiliseringsøvelser.");
        larsAppts.Add(aL4); annaAppts.Add(aL4); anna.Appointments.Add(aL4.Id);

        var aL5 = Appointment.Create(
            new TimeInterval(new DateTime(2025, 12, 8, 10, 0, 0), new DateTime(2025, 12, 8, 11, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        aL5.Complete("Første session efter pause.");
        larsAppts.Add(aL5); annaAppts.Add(aL5); anna.Appointments.Add(aL5.Id);

        var a2 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 27, 14, 0, 0), new DateTime(2026, 5, 27, 15, 0, 0)),
            physio.Id, metteBak.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), metteBAppts, annaAppts);
        a2.Complete("Albuen viser bedring, ny session om 2 uger.");
        metteBAppts.Add(a2); annaAppts.Add(a2); anna.Appointments.Add(a2.Id);

        var a3 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 15, 10, 0, 0), new DateTime(2026, 5, 15, 11, 0, 0)),
            massage.Id, mariaPed.Id, michael.Id, clinic1.Id, massage.GetBasePrice(60), mariaPAppts, michaelAppts);
        a3.Complete("Muskelspændinger i nakke og skuldre behandlet.");
        mariaPAppts.Add(a3); michaelAppts.Add(a3); michael.Appointments.Add(a3.Id);

        var a4 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 30, 10, 0, 0), new DateTime(2026, 5, 30, 11, 0, 0)),
            massage.Id, thomas.Id, frederik.Id, clinic1.Id, massage.GetBasePrice(60), thomasAppts, frederikAppts);
        a4.Complete("Nakkestivhed reduceret markant.");
        thomasAppts.Add(a4); frederikAppts.Add(a4); frederik.Appointments.Add(a4.Id);

        var a5 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 28, 9, 0, 0), new DateTime(2026, 5, 28, 9, 45, 0)),
            acupuncture.Id, sofieA.Id, sara.Id, clinic2.Id, acupuncture.GetBasePrice(45), sofieAAppts, saraAppts);
        a5.Complete("Første akupunkturbehandling gennemført.");
        sofieAAppts.Add(a5); saraAppts.Add(a5); sara.Appointments.Add(a5.Id);

        var a6 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 26, 14, 0, 0), new DateTime(2026, 5, 26, 14, 45, 0)),
            acupuncture.Id, henrik.Id, jonas.Id, clinic3.Id, acupuncture.GetBasePrice(45), henrikAppts, jonasAppts);
        a6.Complete("Rygsmerter afhjulpet med akupunktur.");
        henrikAppts.Add(a6); jonasAppts.Add(a6); jonas.Appointments.Add(a6.Id);

        // ── No-show (afspejler tabet af bookinger på 8-10%) ────────────────────────

        var a7 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 5, 22, 14, 0, 0), new DateTime(2026, 5, 22, 15, 0, 0)),
            physio.Id, oliver.Id, kasper.Id, clinic2.Id, physio.GetBasePrice(60), oliverAppts, kasperAppts);
        a7.NoOneShowed();
        oliverAppts.Add(a7); kasperAppts.Add(a7); kasper.Appointments.Add(a7.Id);

        // ── Aflyste sessioner (afspejler tabet af bookinger på 8-10%) ─────────────

        var a8 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 6, 10, 0, 0), new DateTime(2026, 6, 6, 11, 0, 0)),
            massage.Id, thomas.Id, frederik.Id, clinic1.Id, massage.GetBasePrice(60), thomasAppts, frederikAppts);
        a8.Cancel();
        thomasAppts.Add(a8); frederikAppts.Add(a8); frederik.Appointments.Add(a8.Id);

        var a9 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 2, 9, 0, 0), new DateTime(2026, 6, 2, 10, 0, 0)),
            dietitian.Id, nadia.Id, louise.Id, clinic1.Id, dietitian.GetBasePrice(60), nadiaAppts, louiseAppts);
        a9.Cancel();
        nadiaAppts.Add(a9); louiseAppts.Add(a9); louise.Appointments.Add(a9.Id);

        // ── Kommende bookede sessioner ──────────────────────────────────────────

        var a10 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 3, 9, 0, 0), new DateTime(2026, 6, 3, 10, 0, 0)),
            physio.Id, lars.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), larsAppts, annaAppts);
        larsAppts.Add(a10); annaAppts.Add(a10); anna.Appointments.Add(a10.Id);

        var a11 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 3, 11, 0, 0), new DateTime(2026, 6, 3, 12, 0, 0)),
            physio.Id, anders.Id, anna.Id, clinic1.Id, physio.GetBasePrice(60), andersAppts, annaAppts);
        andersAppts.Add(a11); annaAppts.Add(a11); anna.Appointments.Add(a11.Id);

        var a12 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 1, 14, 0, 0), new DateTime(2026, 6, 1, 15, 0, 0)),
            physio.Id, camilla.Id, kasper.Id, clinic2.Id, physio.GetBasePrice(60), camillaAppts, kasperAppts);
        camillaAppts.Add(a12); kasperAppts.Add(a12); kasper.Appointments.Add(a12.Id);

        var a13 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 4, 9, 0, 0), new DateTime(2026, 6, 4, 9, 45, 0)),
            acupuncture.Id, sofieA.Id, sara.Id, clinic2.Id, acupuncture.GetBasePrice(45), sofieAAppts, saraAppts);
        sofieAAppts.Add(a13); saraAppts.Add(a13); sara.Appointments.Add(a13.Id);

        var a14 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 4, 11, 0, 0), new DateTime(2026, 6, 4, 11, 45, 0)),
            acupuncture.Id, nadia.Id, sara.Id, clinic2.Id, acupuncture.GetBasePrice(45), nadiaAppts, saraAppts);
        nadiaAppts.Add(a14); saraAppts.Add(a14); sara.Appointments.Add(a14.Id);

        var a15 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 4, 13, 0, 0), new DateTime(2026, 6, 4, 14, 0, 0)),
            massage.Id, mariaPed.Id, michael.Id, clinic1.Id, massage.GetBasePrice(60), mariaPAppts, michaelAppts);
        mariaPAppts.Add(a15); michaelAppts.Add(a15); michael.Appointments.Add(a15.Id);

        var a16 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 2, 10, 0, 0), new DateTime(2026, 6, 2, 11, 0, 0)),
            dietitian.Id, emmaD.Id, julie.Id, clinic2.Id, dietitian.GetBasePrice(60), emmaDAppts, julieAppts);
        emmaDAppts.Add(a16); julieAppts.Add(a16); julie.Appointments.Add(a16.Id);

        var a17 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 3, 14, 0, 0), new DateTime(2026, 6, 3, 15, 0, 0)),
            massage.Id, henrik.Id, emma.Id, clinic3.Id, massage.GetBasePrice(60), henrikAppts, emmaAppts);
        henrikAppts.Add(a17); emmaAppts.Add(a17); emma.Appointments.Add(a17.Id);

        var a18 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 3, 14, 0, 0), new DateTime(2026, 6, 3, 14, 45, 0)),
            acupuncture.Id, lena.Id, jonas.Id, clinic3.Id, acupuncture.GetBasePrice(45), lenaAppts, jonasAppts);
        lenaAppts.Add(a18); jonasAppts.Add(a18); jonas.Appointments.Add(a18.Id);

        var a19 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 5, 9, 0, 0), new DateTime(2026, 6, 5, 10, 0, 0)),
            physio.Id, rasmus.Id, peter.Id, clinic1.Id, physio.GetBasePrice(60), rasmusAppts, peterAppts);
        rasmusAppts.Add(a19); peterAppts.Add(a19); peter.Appointments.Add(a19.Id);

        var a20 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 5, 11, 0, 0), new DateTime(2026, 6, 5, 12, 0, 0)),
            physio.Id, ida.Id, peter.Id, clinic1.Id, physio.GetBasePrice(60), idaAppts, peterAppts);
        idaAppts.Add(a20); peterAppts.Add(a20); peter.Appointments.Add(a20.Id);

        var a21 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 5, 13, 0, 0), new DateTime(2026, 6, 5, 14, 0, 0)),
            physio.Id, mikkel.Id, metteLar.Id, clinic3.Id, physio.GetBasePrice(60), mikkelAppts, metteLarAppts);
        mikkelAppts.Add(a21); metteLarAppts.Add(a21); metteLar.Appointments.Add(a21.Id);

        var a22 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 10, 11, 0, 0), new DateTime(2026, 6, 10, 12, 0, 0)),
            physio.Id, camilla.Id, metteLar.Id, clinic3.Id, physio.GetBasePrice(60), camillaAppts, metteLarAppts);
        camillaAppts.Add(a22); metteLarAppts.Add(a22); metteLar.Appointments.Add(a22.Id);

        var a23 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 4, 11, 0, 0), new DateTime(2026, 6, 4, 12, 0, 0)),
            physio.Id, lena.Id, christian.Id, clinic3.Id, physio.GetBasePrice(60), lenaAppts, christianAppts);
        lenaAppts.Add(a23); christianAppts.Add(a23); christian.Appointments.Add(a23.Id);

        var a24 = Appointment.Create(
            new TimeInterval(new DateTime(2026, 6, 8, 9, 0, 0), new DateTime(2026, 6, 8, 10, 0, 0)),
            groupTraining.Id, oliver.Id, peter.Id, clinic1.Id, groupTraining.GetBasePrice(60), oliverAppts, peterAppts);
        oliverAppts.Add(a24); peterAppts.Add(a24); peter.Appointments.Add(a24.Id);

        db.Appointments.AddRange(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23, a24,
            aL1, aL2, aL3, aL4, aL5);
        db.SaveChanges();
    }
}

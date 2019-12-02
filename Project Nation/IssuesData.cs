using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nationhood_Issues
{
    public class IssuesData: MonoBehaviour
    {
        [Flags]public enum Issues
        {
            UnemploymentCrisis =1,
            Corruption = 2,
            WealthInequality = 4,
            OppressiveGovernment =8,
            DebtCrisis =16,
            ImmigrationCrisis=32,
            OrganizedCrime=64,
            AsthmaEpidemic=128,
            SkillsShortage=256,
            CybersecurityVulnerabilities =512,
            Homelessness =1024,
            Pollution = 2048,
            Stagnation = 4096,
            WorkersProtests = 8192,
            CorporationsBailing = 16384,
            BrainDrain = 32768,
            PetrolRiots = 65536,
            BlackMarkets= 131072,
            DiseaseEpidemic = 262144,
            TeacherStrike = 524288,
            HealthCareCrisis = 1048576,
            DrupEpidemic = 2097152,
            TeacherShortage = 4194304,
            DoctorStrike = 8388608,
            ObesityEpidemic = 16777216,
            ViolentCrime = 33554432,
            PoliceState = 67108864,
            BalrubanDilemma = 134217728,
            OverPopulation = 268435456
        }
        public Issues issues;

        //Variables to pull in
        public int unemploymentValue,
            corruptionValue,
            wealthInequalityValue,
            personalControlValue,
            nationalDebtValue,
            illegalImmigrationValue,
            racialTensionValue,
            legalImmigrationValue,
            crimeValue,
            violentCrimeValue,
            pollutionValue,
            healthcareAccessValue,
            educationValue,
            technologyGrantsValue,
            intelligenceServicesValue,
            povertyValue,
            environmentValue,
            corporateTaxValue,
            importTariffsValue,
            productivityValue,
            averageWageValue,
            laborLawsValue,
            workSafetyLawsValue,
            familyLeaveLawsValue,
            incomeTaxValue,
            luxuryTaxValue,
            oilPriceValue,
            salesTaxValue,
            accessToHealthCareValue,
            fundingForStateSchoolsValue,
            fundingForStateHospitalsValue,
            narcoticsAvailabilityValue,
            agriculturalSubsidiesValue,
            gdpValue,
            stateHospitalsValue,
            armedPoliceValue,
            cctvSurveillanceValue,
            communityPolicingValue,
            personalLibertiesValue,
            populationValue;

        //Base Values for High/Low Variables
        //Possible need for individual values per issue
        public int unemploymentCrisis_Unemployment_HighValue,
            corruption_Corruption_HighValue,
            wealthInequality_WealthInequality_HighValue,
            oppressiveGovernment_PersonalControl_HighValue,
            debtCrisis_NationalDebt_HighValue,
            immigrationCrisis_IllegalImmigration_HighValue,
            immigrationCrisis_HighValue,
            organizedCrime_HighValue,
            asthmaEpidemic_HighValue, //Calculated as Pollution - Heath Care Access
            skillShortage_LowValue,
            cybersecurityVulnerabilities_IntelligenceServices_LowValue,
            homelessness_Poverty_HighValue,
            homelessness_Unemployment_HighValue,
            pollution_Environment_LowValue,
            workerProtests_AverageWages_LowValue,
            workerProtests_PovertyAndUnemployment_HighValue,
            corporationsBailing_HighValue,
            brainDrain_IncomeTax_HighValue,
            brainDrain_LuxuryTaxes_HighValue,
            petrolRiots_OilPrices_HighValue,
            petrolRiots_Poverty_HighValue,
            blackMarkets_HighValue,
            diseaseEpidemic_Poverty_HighValue,
            diseaseEpidemic_AccessToHealthCare_LowValue,
            teacherStrike_FundingForStateSchools_LowValue,
            healthCareCrisis_FundingForStateHospitals_LowValue,
            drugEpidemic_Narcotics_HighValue,
            drugEpidemic_Unemployment_HighValue,
            teacherShortage_FundingForStateSchools_LowValue,
            teacherShortage_AverageWages_LowValue,
            teacherShortage_Immigration_HighValue,
            doctorStrike_FundingForStateHospitals_LowValue,
            doctorStrike_AverageWages_LowValue,
            ObesityEpidemic_AgriculturalSusidies_HighValue,
            ObesityEpidemic_GDP_HighValue,
            ObesityEpidemic_StateHospitals_LowValue,
            violentCrime_ArmedPolice_LowValue,
            violentCrime_CCTV_LowValue,
            violentCrime_CommunityPolicing_LowValue,
            violentCrime_Poverty_HighValue,
            policeState_ArmedPolice_HighValue,
            policeState_CCTV_HighValue,
            policeState_PersonalLiberties_LowValue,
            overPopulation_HighValue;

        public float balrubanDilemma_Resource_FiftyPercent = .5f;
        public float exports;
        public float[] resources;
    } 
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nationhood_Issues
{
    public class Issues : MonoBehaviour
    {
        public void IssueChecker()
        {
            var issues = EnumUtilities.GetValues<IssuesData.Issues>();
            var data = FindObjectOfType<IssuesData>();
            int issueCheck = 0;

            //Unemployment Crisis
            issueCheck += IssueCheck(issues[0],
                ValueGreaterThanEqualTo(data.unemploymentValue, data.unemploymentCrisis_Unemployment_HighValue));

            //Corruption
            issueCheck += IssueCheck(issues[1],
                ValueGreaterThanEqualTo(data.corruptionValue, data.corruption_Corruption_HighValue));

            //Wealth Inequality
            issueCheck += IssueCheck(issues[2],
                ValueGreaterThanEqualTo(data.wealthInequalityValue, data.wealthInequality_WealthInequality_HighValue));

            //Oppressive Government
            issueCheck += IssueCheck(issues[3],
                ValueGreaterThanEqualTo(data.personalControlValue, data.oppressiveGovernment_PersonalControl_HighValue));

            //Debt Crisis
            issueCheck += IssueCheck(issues[4],
                ValueGreaterThanEqualTo(data.nationalDebtValue, data.debtCrisis_NationalDebt_HighValue));

            //Immigration Crisis
            issueCheck += IssueCheck(issues[5],
                ValueGreaterThanEqualTo(data.illegalImmigrationValue, data.immigrationCrisis_IllegalImmigration_HighValue),
                ValueGreaterThanEqualTo(data.racialTensionValue + data.legalImmigrationValue, data.immigrationCrisis_HighValue));

            //Organized Crime
            issueCheck += IssueCheck(issues[6],
                ValueGreaterThanEqualTo(data.crimeValue + data.violentCrimeValue + data.corruptionValue,
                    data.organizedCrime_HighValue));

            //Asthma Epidemic
            issueCheck += IssueCheck(issues[7],
                ValueGreaterThanEqualTo(data.pollutionValue - data.healthcareAccessValue, data.asthmaEpidemic_HighValue));

            //Skills Shortage
            issueCheck += IssueCheck(issues[8],
                ValueLessThanEqualTo(data.educationValue + data.technologyGrantsValue, data.skillShortage_LowValue));

            //Cybersecurity Vulnerabilities
            issueCheck += IssueCheck(issues[9],
                ValueLessThanEqualTo(data.intelligenceServicesValue, data.cybersecurityVulnerabilities_IntelligenceServices_LowValue));

            //Homelessness
            issueCheck += IssueCheck(issues[10],
                ValueGreaterThanEqualTo(data.povertyValue, data.homelessness_Poverty_HighValue),
                ValueGreaterThanEqualTo(data.unemploymentValue, data.homelessness_Unemployment_HighValue));

            //Pollution
            issueCheck += IssueCheck(issues[11],
                ValueLessThanEqualTo(data.environmentValue, data.pollution_Environment_LowValue));

            //Stagnation
            issueCheck += IssueCheck(issues[12],
                ValueGreaterThanEqualTo(data.corporateTaxValue + data.importTariffsValue, data.productivityValue));

            //Workers Protests
            issueCheck += IssueCheck(issues[13],
                ValueGreaterThanEqualTo(data.averageWageValue, data.workerProtests_AverageWages_LowValue),
                ValueGreaterThanEqualTo(data.povertyValue + data.unemploymentValue, data.workerProtests_PovertyAndUnemployment_HighValue));

            //Corporations Bailing
            issueCheck += IssueCheck(issues[14],
                ValueGreaterThanEqualTo(data.laborLawsValue + data.corporateTaxValue +
                                        data.workSafetyLawsValue + data.familyLeaveLawsValue, data.corporationsBailing_HighValue));

            //Brain Drain
            issueCheck += IssueCheck(issues[15],
                ValueGreaterThanEqualTo(data.incomeTaxValue, data.brainDrain_IncomeTax_HighValue),
                ValueGreaterThanEqualTo(data.luxuryTaxValue, data.brainDrain_LuxuryTaxes_HighValue));

            //Petrol Riots
            issueCheck += IssueCheck(issues[16],
                ValueGreaterThanEqualTo(data.oilPriceValue, data.petrolRiots_OilPrices_HighValue),
                ValueGreaterThanEqualTo(data.povertyValue, data.petrolRiots_Poverty_HighValue));

            //Black Markets
            issueCheck += IssueCheck(issues[17],
                ValueGreaterThanEqualTo(data.incomeTaxValue + data.corporateTaxValue + data.salesTaxValue, data.blackMarkets_HighValue));

            //Disease Epidemic
            issueCheck += IssueCheck(issues[18],
                ValueGreaterThanEqualTo(data.povertyValue, data.diseaseEpidemic_Poverty_HighValue),
                ValueLessThanEqualTo(data.accessToHealthCareValue, data.diseaseEpidemic_AccessToHealthCare_LowValue));

            //Teacher's Strike
            issueCheck += IssueCheck(issues[19],
                ValueLessThanEqualTo(data.fundingForStateSchoolsValue, data.teacherStrike_FundingForStateSchools_LowValue));

            //Health Care Crisis
            issueCheck += IssueCheck(issues[20],
                ValueLessThanEqualTo(data.fundingForStateHospitalsValue, data.healthCareCrisis_FundingForStateHospitals_LowValue));

            //Drug Epidemic
            issueCheck += IssueCheck(issues[21],
                ValueGreaterThanEqualTo(data.narcoticsAvailabilityValue, data.drugEpidemic_Narcotics_HighValue),
                ValueGreaterThanEqualTo(data.unemploymentValue, data.drugEpidemic_Unemployment_HighValue));

            //Teacher Shortage
            issueCheck += IssueCheck(issues[22],
                ValueGreaterThanEqualTo(data.legalImmigrationValue, data.teacherShortage_Immigration_HighValue),
                ValueLessThanEqualTo(data.fundingForStateSchoolsValue, data.teacherShortage_FundingForStateSchools_LowValue),
                ValueLessThanEqualTo(data.averageWageValue, data.teacherShortage_AverageWages_LowValue));

            //Doctor's Strike
            issueCheck += IssueCheck(issues[23],
                ValueLessThanEqualTo(data.averageWageValue, data.doctorStrike_AverageWages_LowValue),
                ValueLessThanEqualTo(data.fundingForStateHospitalsValue, data.doctorStrike_FundingForStateHospitals_LowValue));

            //Obesity Epidemic
            issueCheck += IssueCheck(issues[24],
                ValueGreaterThanEqualTo(data.agriculturalSubsidiesValue, data.ObesityEpidemic_AgriculturalSusidies_HighValue),
                ValueGreaterThanEqualTo(data.gdpValue, data.ObesityEpidemic_GDP_HighValue),
                ValueLessThanEqualTo(data.stateHospitalsValue, data.ObesityEpidemic_StateHospitals_LowValue));

            //Violent Crime
            issueCheck += IssueCheck(issues[25],
                ValueGreaterThanEqualTo(data.povertyValue, data.violentCrime_Poverty_HighValue),
                ValueLessThanEqualTo(data.armedPoliceValue, data.violentCrime_ArmedPolice_LowValue),
                ValueLessThanEqualTo(data.cctvSurveillanceValue, data.violentCrime_CCTV_LowValue),
                ValueLessThanEqualTo(data.communityPolicingValue, data.violentCrime_CommunityPolicing_LowValue));

            //Police State
            issueCheck += IssueCheck(issues[26],
                ValueGreaterThanEqualTo(data.armedPoliceValue, data.policeState_ArmedPolice_HighValue),
                ValueGreaterThanEqualTo(data.cctvSurveillanceValue, data.policeState_CCTV_HighValue),
                ValueLessThanEqualTo(data.personalLibertiesValue, data.policeState_PersonalLiberties_LowValue));

            //Balruban Dilemma
            issueCheck += IssueCheck(issues[27],
                ValueGreaterThanEqualTo_Float(data.unemploymentValue, data.balrubanDilemma_Resource_FiftyPercent));
        }

        int IssueCheck(IssuesData.Issues issue, params bool[] parameters)
        {
            if (AllValuesTrue(parameters))
            {
                return (int)issue;
            }
            return 0;
        }

        public static bool ValueGreaterThanEqualTo(int value, int highValue)
        {
            return value >= highValue;
        }

        public static bool ValueGreaterThanEqualTo_Float(float value, float highValue)
        {
            return value >= highValue;
        }

        public static bool ValueLessThanEqualTo(int value, int lowValue)
        {
            return value <= lowValue;
        }

        public static bool AllValuesTrue(params bool[] values)
        {
            return values.All(value => value);
        }
    }
}

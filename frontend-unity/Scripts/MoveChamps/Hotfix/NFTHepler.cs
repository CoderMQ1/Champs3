using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Generate;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class NFTHepler
    {
         public static RoleData RadomNft(int level)
        {
            string[] heroName = new[] { "C_1_1", "C_1_2","C_1_3","C_1_4","C_1_5"};
            var heroIndex = Random.Range(0, heroName.Length);


            int talentId = heroIndex == 0 ? 0 : Random.Range(1, 5) +((heroIndex - 1) * 4);

            RoleTalent roleTalent = ExcelConfig.RoleTalentTable.Data.Find(config =>
            {
                return config.Id == talentId;
            });

            if (roleTalent == null)
            {
                LogKit.E($"Not Role Talent {heroIndex} {talentId}");
            }

            int baseAtt = 10 + (level * 4);

            int runAtt = Random.Range(1, baseAtt - 2);

            int swimAtt = Random.Range(1, baseAtt - runAtt - 1);

            int clmbAtt = Random.Range(1, baseAtt - runAtt - swimAtt);

            int flyAtt = Random.Range(1, baseAtt - runAtt - swimAtt - clmbAtt + 1);
            int runTalet = 0;
            int swimTalet = 0;
            int clmbTalet = 0;
            int flyTalet = 0;

            if (roleTalent != null)
            {
                switch (roleTalent.AttriType)
                {
                    case 1:
                        runAtt += roleTalent.BaseSpeedAdd;
                    
                        runTalet = (int)(runAtt * roleTalent.PercentageSpeedAdd / 100f);
                        break;
                    case 2:
                        swimAtt += roleTalent.BaseSpeedAdd;
                    
                        swimTalet = (int)(swimAtt * roleTalent.PercentageSpeedAdd / 100f);
                    
                        break;
                    case 3:
                        clmbAtt += roleTalent.BaseSpeedAdd;
                    
                        clmbTalet = (int)(clmbAtt * roleTalent.PercentageSpeedAdd / 100f);
                        break;
                    case 4:
                        flyAtt += roleTalent.BaseSpeedAdd;
                    
                        flyTalet = (int)(flyAtt * roleTalent.PercentageSpeedAdd / 100f);
                        break;
                
                }

            }

            RoleData roleData = new RoleData()
            {
                NftID = 1,
                Character = heroName[heroIndex],
                TalentId = talentId,
                Level = level,
                Energy = (heroIndex * 1) * 100,
                Attributes = new List<RoleAttribute>()
                {
                    new RoleAttribute() { AttriType = 1, AttriValue = runAtt, TalentValue = runTalet, Speed = 3 + Random.Range(0.5f, 1)},
                    new RoleAttribute() { AttriType = 2, AttriValue = swimAtt, TalentValue = swimTalet, Speed = 3 + Random.Range(0.5f, 1) },
                    new RoleAttribute() { AttriType = 3, AttriValue = clmbAtt, TalentValue = clmbTalet, Speed = 3 + Random.Range(0.5f, 1)},
                    new RoleAttribute() { AttriType = 4, AttriValue = flyAtt, TalentValue = flyTalet, Speed = 3 + Random.Range(0.5f, 1)},
                }
            };
            return roleData;
        }
    }
}
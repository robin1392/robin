using System;


namespace Table.Data
{
    public class TDataBoxProductInfo : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public int boxId { get; set; }
        public int userRankGrade { get; set; }
        public int gold { get; set; }
        public int dia { get; set; }
        public int rewardCardGradeType1 { get; set; }
        public int rewardCardGradeCnt1 { get; set; }
        public int[] rewardCardValue1 { get; set; }
        public bool rewardIsProbability1 { get; set; }
        public int rewardProbability1 { get; set; }
        public int rewardCardGradeType2 { get; set; }
        public int rewardCardGradeCnt2 { get; set; }
        public int[] rewardCardValue2 { get; set; }
        public bool rewardIsProbability2 { get; set; }
        public int rewardProbability2 { get; set; }
        public int rewardCardGradeType3 { get; set; }
        public int rewardCardGradeCnt3 { get; set; }
        public int[] rewardCardValue3 { get; set; }
        public bool rewardIsProbability3 { get; set; }
        public int rewardProbability3 { get; set; }
        public int rewardCardGradeType4 { get; set; }
        public int rewardCardGradeCnt4 { get; set; }
        public int[] rewardCardValue4 { get; set; }
        public bool rewardIsProbability4 { get; set; }
        public int rewardProbability4 { get; set; }
        public int rewardCardGradeType5 { get; set; }
        public int rewardCardGradeCnt5 { get; set; }
        public int[] rewardCardValue5 { get; set; }
        public bool rewardIsProbability5 { get; set; }
        public int rewardProbability5 { get; set; }
        public int buyLimitCnt { get; set; }
        public bool isUse { get; set; }


        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            boxId = int.Parse(cols[2]);
            userRankGrade = int.Parse(cols[3]);
            gold = int.Parse(cols[4]);
            dia = int.Parse(cols[5]);
            rewardCardGradeType1 = int.Parse(cols[6]);
            rewardCardGradeCnt1 = int.Parse(cols[7]);
            rewardCardValue1 = Array.ConvertAll(cols[8].Split('|'), s => int.Parse(s));
            rewardIsProbability1 = bool.Parse(cols[9]);
            rewardProbability1 = int.Parse(cols[10]);
            if (cols[11].Length != 0) rewardCardGradeType2 = int.Parse(cols[11]);
            if (cols[12].Length != 0) rewardCardGradeCnt2 = int.Parse(cols[12]);
            if (cols[13].Length != 0) rewardCardValue2 = Array.ConvertAll(cols[13].Split('|'), s => int.Parse(s));
            if (cols[14].Length != 0) rewardIsProbability2 = bool.Parse(cols[14]);
            if (cols[15].Length != 0) rewardProbability2 = int.Parse(cols[15]);
            if (cols[16].Length != 0) rewardCardGradeType3 = int.Parse(cols[16]);
            if (cols[17].Length != 0) rewardCardGradeCnt3 = int.Parse(cols[17]);
            if (cols[18].Length != 0) rewardCardValue3 = Array.ConvertAll(cols[18].Split('|'), s => int.Parse(s));
            if (cols[19].Length != 0) rewardIsProbability3 = bool.Parse(cols[19]);
            if (cols[20].Length != 0) rewardProbability3 = int.Parse(cols[20]);
            if (cols[21].Length != 0) rewardCardGradeType4 = int.Parse(cols[21]);
            if (cols[22].Length != 0) rewardCardGradeCnt4 = int.Parse(cols[22]);
            if (cols[23].Length != 0) rewardCardValue4 = Array.ConvertAll(cols[23].Split('|'), s => int.Parse(s));
            if (cols[24].Length != 0) rewardIsProbability4 = bool.Parse(cols[24]);
            if (cols[25].Length != 0) rewardProbability4 = int.Parse(cols[25]);
            if (cols[26].Length != 0) rewardCardGradeType5 = int.Parse(cols[26]);
            if (cols[27].Length != 0) rewardCardGradeCnt5 = int.Parse(cols[27]);
            if (cols[28].Length != 0) rewardCardValue5 = Array.ConvertAll(cols[28].Split('|'), s => int.Parse(s));
            if (cols[29].Length != 0) rewardIsProbability5 = bool.Parse(cols[29]);
            if (cols[30].Length != 0) rewardProbability5 = int.Parse(cols[30]);
            buyLimitCnt = int.Parse(cols[31]);
            isUse = bool.Parse(cols[32]);
        }
    }
}
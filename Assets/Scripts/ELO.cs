public static class ELO {

    static int K(float score)
    {
        if (score >= 2400)
            return 15;
        else if (score >= 1500)
            return 30;
        else
            return 50;
    }

    static float score_change_koef(float score, float another_player_score, bool win) 
    {
        float mat = (float)(1 / (1 + System.Math.Pow(10, ((another_player_score - score) / 400))));
        return win ? 1 - mat : 0 - mat;
    }

    public static int[] new_scores(int[] scores_by_place)
    {
        int[] result_score = new int[4];  
        for(int self=0; self<4; self++)
        {
            float koef = 0;
            for (int another = 0; another < 4; another++)
            {
                if (self == another)
                    continue;
                bool isWin = self < another;
                koef += score_change_koef(scores_by_place[self], scores_by_place[another], isWin);
            }
            result_score[self] = scores_by_place[self] + (int)(K(scores_by_place[self]) * (koef/3));
        }
        return result_score;
    }

}
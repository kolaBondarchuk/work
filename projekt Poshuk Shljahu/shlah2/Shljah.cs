using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Shljah
{
    static int n = 600000;
    static int[] i;
    static int max_value,
        ibegin, iend,
        j = 0;
    static double[,] m_sumizn;
    static HashSet<int> set = new HashSet<int>();
    static int[][] dz_sort_shljah = new int[n][];
    static double[] dz_suma = new double[n];

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="matrix_sumizn">Матриця суміжності</param>
    /// <param name="max">Максимальне значення кожного циклу</param>
    /// <param name="begin">Початковий пункт</param>
    /// <param name="end">Кінцевий пункт</param>
    public static void vvid(double[,] matrix_sumizn, int max, int begin, int end)
    {
        ibegin = begin;
        iend = end;
        m_sumizn = matrix_sumizn;
        max_value = max;
        i = new int[max + 1];
        for (int a = 0; a < n; a++)
            dz_sort_shljah[a] = new int[max + 1];
        f(ibegin);
    }
    static int l = 0;
    /// <summary>
    /// Імітує вложені цикли
    /// </summary>
    /// <param name="Level">Рівень глубини</param>
    public static void f(int Level)
    {
        set.Add(Level);
        dz_sort_shljah[j][l++] = Level + 1;
        for (i[Level] = 0; i[Level] < max_value; ++i[Level])
        {
            if (((m_sumizn[i[Level], Level] > 0) && (!set.Contains(i[Level]))))
            {
                if (i[Level] == iend)
                {
                    dz_sort_shljah[j++][l] = iend + 1;
                    for (int k = 0; k < l + 1; k++)
                        dz_sort_shljah[j][k] = dz_sort_shljah[j - 1][k];
                    continue;
                }
                else
                {
                    f(i[Level]);
                }
            }
        }
        set.Remove(Level);
        dz_sort_shljah[j][l] = 0;
        l--;
    }

    static void suma()
    {
        for (int a = 0; a < j; a++)
            for (int b = 0; b < max_value; b++)
                if ((dz_sort_shljah[a][b]  > 0) && ((int)dz_sort_shljah[a][b + 1] > 0))
                    dz_suma[a] += m_sumizn[(int)dz_sort_shljah[a][b] - 1, (int)dz_sort_shljah[a][b + 1] - 1];
    }

    static void sortovanie()
    {
        double d;
        int[] D;
        for (int a = 0; a < j - 1; a++)
            for (int b = a + 1; b < j; b++)
                if (dz_suma[a] > dz_suma[b])
                {
                    d = dz_suma[a];
                    dz_suma[a] = dz_suma[b];
                    dz_suma[b] = d;
                    D = dz_sort_shljah[a];
                    dz_sort_shljah[a] = dz_sort_shljah[b];
                    dz_sort_shljah[b] = D;
                }
    }

    /// <summary>
    /// Вивід
    /// </summary>
    /// <param name="sort_shljah">Відсортований масив шляхів</param>
    /// <param name="dovzina">їхні довжини</param>
    /// <param name="n_sljahiv">кількість шляхів</param>
    public static void vuvid(out int[][] sort_shljah, out double[] dovzina, out int n_sljahiv)
    {
        suma();
        sortovanie();
        sort_shljah = dz_sort_shljah;
        dovzina = dz_suma;
        n_sljahiv = j;
    }
}

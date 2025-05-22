using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;


namespace CLPC
{
    internal class CLPC_Main
    {
        public static int[] compound_nmr_protons = new int[1024];
        public static int[] compound_nmr_carbons = new int[1024];

        public static int[] compound_c_composition = new int[1024];
        public static int[] compound_h_composition = new int[1024];


        static void Main()
        {
            const double version = 0.55;
            string[] file_args = new string[1];
            Console.WriteLine($"======CPLC======\n--Version: {version}--\n--By: Rim032--\n");

            Console.WriteLine("Drag or drop your the .txt file of your research paper.");
            file_args[0] = format_file_location(Console.ReadLine());


            if (file_args == null || !File.Exists(file_args[0]))
            {
                return;
            }
            string paper_lines = File.ReadAllText(file_args[0]);

            string[] proton_nmr_lines = obtain_hnmr_data(paper_lines);
            check_hnmr_data(proton_nmr_lines);
            string[] carbon_nmr_lines = obtain_cnmr_data(paper_lines);
            check_cnmr_data(carbon_nmr_lines);


            string[] rough_compound_formulas = obtain_compound_formulas(paper_lines);
            rufisfy_compound_formulas(rough_compound_formulas);
            compare_compounds_with_nmr(rough_compound_formulas, proton_nmr_lines, carbon_nmr_lines);

            Console.ReadLine();
        }

        private static void check_hnmr_data(string[] nmr_data)
        {
            if (nmr_data == null)
            {
                return;
            }

            for (int l = 0; l < nmr_data.Length; l++)
            {
                try
                {
                    char[] nmr_data_chars = nmr_data[l].ToCharArray();
                    for (int c = 0; c < nmr_data_chars.Length; c++)
                    {
                        if (c + 1 < nmr_data_chars.Length && (int)nmr_data_chars[c] > 46 && (int)nmr_data_chars[c] < 58 && nmr_data_chars[c + 1] == 'H')
                        {
                            compound_nmr_protons[l] = compound_nmr_protons[l] + 1;
                        }
                    }
                }
                catch (Exception error) { }
                //Is this a lazy solution? Yes.
            }
        }

        private static void check_cnmr_data(string[] nmr_data)
        {
            if (nmr_data == null)
            {
                return;
            }

            for (int l = 0; l < nmr_data.Length; l++)
            {
                try
                {
                    char[] nmr_data_chars = nmr_data[l].ToCharArray();
                    for (int c = 0; c < nmr_data_chars.Length; c++)
                    {
                        if (c + 1 < nmr_data_chars.Length && (int)nmr_data_chars[c] > 46 && (int)nmr_data_chars[c] < 58 && nmr_data_chars[c + 1] == '.')
                        {
                            compound_nmr_carbons[l] = compound_nmr_carbons[l] + 1;
                        }
                    }
                }
                catch (Exception error) { }
            }
        }

        private static void rufisfy_compound_formulas(string[] compound_data)
        {
            for (int i = 0; i < compound_data.Length; i++)
            {
                try
                {
                    char[] rough_formula_chars = compound_data[i].ToCharArray();
                    for (int j = 0; j < rough_formula_chars.Length; j++)
                    {
                        if (rough_formula_chars[j] == 'C') //Bad programming assumption here.
                        {
                            int first_digit_c_char = 0;
                            if ((int)rough_formula_chars[j + 2] > 48 && (int)rough_formula_chars[j + 2] < 58)
                            {
                                first_digit_c_char = ((int)rough_formula_chars[j + 1] - 48) + 10;
                            }
                            compound_c_composition[i] = first_digit_c_char + ((int)rough_formula_chars[j + 2] - 48);
                        }
                        if (rough_formula_chars[j] == 'H')
                        {
                            int first_digit_h_char = 0;
                            if ((int)rough_formula_chars[j + 2] > 48 && (int)rough_formula_chars[j + 2] < 58)
                            {
                                first_digit_h_char = ((int)rough_formula_chars[j + 1] - 48) + 10;
                            }
                            compound_h_composition[i] = first_digit_h_char + ((int)rough_formula_chars[j + 2] - 48);
                        }
                    }
                }
                catch (Exception error) { }
            }
        }

        private static string[] obtain_compound_formulas(string file)
        {
            string[] compound_formula_lines = new string[1024];

            const string compound_regex_key = @"calculated for(.*?)\[";
            Regex compound_regex = new Regex(compound_regex_key, RegexOptions.None);

            if (compound_regex != null)
            {
                int compound_counter = 0;
                foreach (Match hnmr_match in compound_regex.Matches(file))
                {
                    compound_formula_lines[compound_counter] = (hnmr_match.ToString());
                    compound_counter++;
                }
            }

            return compound_formula_lines;
        }

        private static string[] obtain_hnmr_data(string file)
        {
            string[] hnmr_data_lines = new string[1024];

            const string hnmr_regex_key = @"1H NMR(.*?)\)\.";
            Regex hnmr_regex = new Regex(hnmr_regex_key, RegexOptions.None);

            if (hnmr_regex != null)
            {
                int hnmr_counter = 0;
                foreach (Match hnmr_match in hnmr_regex.Matches(file))
                {
                    hnmr_data_lines[hnmr_counter] = (hnmr_match.ToString());
                    hnmr_counter++;
                }
            }

            return hnmr_data_lines;
        }

        private static string[] obtain_cnmr_data(string file)
        {
            string[] cnmr_data_lines = new string[1024];

            const string cnmr_regex_key = @"13C NMR(.*?)\. ";
            Regex cnmr_regex = new Regex(cnmr_regex_key, RegexOptions.None);

            if (cnmr_regex != null)
            {
                int cnmr_counter = 0;
                foreach (Match cnmr_match in cnmr_regex.Matches(file))
                {
                    cnmr_data_lines[cnmr_counter] = (cnmr_match.ToString());
                    cnmr_counter++;
                }
            }

            return cnmr_data_lines;
        }

        private static void compare_compounds_with_nmr(string[] compound_formulas, string[] proton_nmr_lines, string[] carbon_nmr_lines)
        {
            for (int a = 0; a < 1024; a++)
            {
                if (compound_formulas[a] != null && proton_nmr_lines != null && carbon_nmr_lines != null && compound_h_composition[a] != 0 &&
                compound_c_composition[a] != 0 && compound_nmr_carbons[a] != 0 && compound_nmr_protons[a] != 0)
                {
                    Console.Write($"\n\n--------------------------------------------------------------------------------\nINTERNAL COMPOUND INDEX: {a + 1}\n{compound_formulas[a]}\n\n{proton_nmr_lines[a]}\n\n{carbon_nmr_lines[a]}\n\n");
                    if (compound_c_composition[a] != compound_nmr_carbons[a])
                    {
                        Console.Write("\nWARNING: # of spectra CARBONS isn't equal to the formula's carbons!");
                    }

                    if (compound_h_composition[a] != compound_nmr_protons[a])
                    {
                        Console.Write("\nWARNING: # of spectra PROTONS isn't equal to the formula's protons!\n");
                    }
                    Console.Write("--------------------------------------------------------------------------------\n\n");
                }
            }
        }

        private static string format_file_location(string file)
        {
            string final_file = "";
            if (file == null)
            {
                return final_file;
            }

            string[] file_arr = file.Split("\"");
            for (int i = 0; i < file_arr.Length; i++)
            {
                if (file_arr[i] != "\"")
                {
                    final_file += file_arr[i];
                }
            }

            return final_file;
        }
    }
}
using System;
using System.IO;
using System.Reflection;

namespace source2MMBatch
{
    static class Extensions
    {
        public static string Color { get; set; }
        public static string Normal { get; set; }
        public static string Rough { get; set; }
        public static string FileType { get; set; }
    }
    class Program
    {
        public const string version = "0.1";
        static void Main(string[] args)
        {
            
            string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            Extensions.FileType = ".jpg";
            Extensions.Color = "*_Color";
            Extensions.Normal = "*_Normal";
            Extensions.Rough = "*_Roughness";

            string input;
            Console.WriteLine("Trist's Source 2 VMAT batch V" + version);
            Console.WriteLine("If this is not run from your base material directory, please close and restart after doing so.");
            
            //Gross, but it works.
            Console.WriteLine("Please enter your file extension, or leave blank to use default. Default: [" + Extensions.FileType + "]");
            input = Console.ReadLine();
            if (input.Length > 0 && !input.Contains("."))
                input = "." + input;
            if (!input.Equals(""))
                Extensions.FileType = input;
            
            Console.WriteLine("Please enter your color extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Color + "]");
            input = Console.ReadLine();
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            if (!input.Equals(""))
                Extensions.Color = input;

            Console.WriteLine("Please enter your normal extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Normal + "]");
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            input = Console.ReadLine();
            if (!input.Equals(""))
                Extensions.Normal = input;

            Console.WriteLine("Please enter your rough extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Rough + "]");
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            input = Console.ReadLine();
            if (!input.Equals(""))
                Extensions.Rough = input;

            Directory.CreateDirectory(path + "\\generated_vmat");

            MakeMatRec(path, path);            
            //Console.WriteLine(path);
            //MakeVMFSimple(path, "");            
        }
        
        public static void MakeMatRec(string path, string base_path)
        {
            //Recursively search the path to find all leaf files in child directories
            string[] directories = Directory.GetDirectories(path);
            foreach(string directory in directories)
            {
                MakeMatRec(directory, base_path);                
            }

            string folder = Path.GetFileName(path);
            if (folder.Equals("generated_vmat"))
                return;
            
            //Search through to find all files matching the color regex
            //could be cleaned later, pretty much a hack. I don't think
            //anybody will make a vmat that won't include color at the very least
            string[] files = Directory.GetFiles(path,Extensions.Color+Extensions.FileType);
            foreach (string file in files)
            {
                string file_name = Path.GetFileName(file);

                string new_path = "materials/" + folder + ((folder.Length > 0) ? "/" : "");
                //string color = "materials/default/default_color.tga";   //_d
                string color = new_path + file_name;
                string normal = "materials/default/default_normal.tga"; //_n
                string rough = "materials/default/default_rough.tga";   //_s

                string[] normal_search = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*",""), Extensions.Normal.Replace("*", "")));
                if (normal_search.Length > 0)
                    normal = new_path + Path.GetFileName(normal_search[0]);
                string[] rough_search  = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*", ""), Extensions.Rough.Replace("*", "")));
                if(rough_search.Length > 0)    
                    rough = new_path + Path.GetFileName(rough_search[0]);

                MakeVMFSimple(base_path + "\\generated_vmat\\", file_name.Replace(Extensions.Color.Replace("*",""),"").Replace(Extensions.FileType,"") + ".vmat", color, normal, rough);
            }
        }

        public static void MakeVMFSimple(string path, string file_name, string color, string normal, string rough)
        {
            //string color = "materials/default/default_color.tga";   //_d
            //string normal = "materials/default/default_normal.tga"; //_n
            //string rough = "materials/default/default_rough.tga";   //_s
            string template_simple = "//THIS FILE WAS GENERATED BY TRIST'S BATCH TOOL \n" +
            "Layer0\n" +
            "{\n" +
            "	shader \"vr_simple.vfx\"\n" +
            "	g_flModelTintAmount \"1.000\"\n" +
            "	g_vColorTint \"[1.000000 1.000000 1.000000 0.000000]\"\n" +
            "	TextureColor \"" + color + "\"\n" +
            "	g_flFadeExponent \"1.000\"\n" +
            "	g_bFogEnabled \"1\"\n" +
            "	g_flDirectionalLightmapMinZ \"0.050\"\n" +
            "	g_flDirectionalLightmapStrength \"1.000\"\n" +
            "	g_flMetalness \"0.000\"\n" +
            "	TextureNormal \"" + normal + "\"\n" +
            "	TextureRoughness \"" + rough + "\"\n" +
            "	g_nScaleTexCoordUByModelScaleAxis \"0\"\n" +
            "	g_nScaleTexCoordVByModelScaleAxis \"0\"\n" +
            "	g_vTexCoordOffset \"[0.000 0.000]\"\n" +
            "	g_vTexCoordScale \"[1.000 1.000]\"\n" +
            "	g_vTexCoordScrollSpeed \"[0.000 0.000]\"\n" +
            "}";
            File.Create(path + file_name).Close();
            File.WriteAllText(path + file_name, template_simple);
        }
        public static void MakeVMFComplex(string path, string file, string color, string normal, string rough)
        {
            string template_complex = "//THIS FILE WAS GENERATED BY TRIST'S BATCH TOOL \n" +
            "Layer0\n" +
            "{\n" +
            "	shader \"vr_complex.vfx\"\n" +
            "	g_flAmbientOcclusionDirectDiffuse \"0.000\"\n" +
            "	g_flAmbientOcclusionDirectSpecular \"0.000\"\n" +
            "	TextureAmbientOcclusion \"materials/default/default_ao.tga\"\n" +
            "	g_flModelTintAmount \"1.000\"\n" +
            "	g_vColorTint \"[1.000000 1.000000 1.000000 0.000000]\"\n" +
            "	TextureColor \"" + color + "\"\n" +
            "	g_flFadeExponent \"1.000\"\n" +
            "	g_bFogEnabled \"1\"\n" +
            "	g_flDirectionalLightmapMinZ \"0.050\"\n" +
            "	g_flDirectionalLightmapStrength \"1.000\"\n" +
            "	g_flMetalness \"0.000\"\n" +
            "	TextureNormal \"" + normal + "\"\n" +
            "	TextureRoughness \"" + rough + "\"\n" +
            "	g_nScaleTexCoordUByModelScaleAxis \"0\"\n" +
            "	g_nScaleTexCoordVByModelScaleAxis \"0\"\n" +
            "	g_vTexCoordOffset \"[0.000 0.000]\"\n" +
            "	g_vTexCoordScale \"[1.000 1.000]\"\n" +
            "	g_vTexCoordScrollSpeed \"[0.000 0.000]\"\n" +
            "}";
        }
    }
}

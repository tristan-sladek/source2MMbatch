using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace source2MMBatch
{
    static class Extensions
    {
        public static string Color { get; set; }
        public static string Normal { get; set; }
        public static string Rough { get; set; }
        public static string Ambient { get; set; }
        public static string Metal { get; set; }
        public static string Height { get; set; }
        public static string FileType { get; set; }
        public static string Shader { get; set; }
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
            Extensions.Ambient = "*_AmbientOcclusion";
            Extensions.Metal = "*_Metalness";
            Extensions.Height = "*_Displacement";
            Extensions.Shader = "simple";

            string input;
            Console.WriteLine("Trist's Source 2 VMAT batch V" + version);
            Console.WriteLine("If this is not run from your base material directory, please close and restart after doing so.");

            //Gross, but it works.
            Console.WriteLine("Please enter your preferred shader, or leave blank to use default. Default: [" + Extensions.Shader + "]");
            Console.WriteLine("Supported: simple, complex");

            //this will make more sense if I include more shader types.
            List<string> supported = new List<string>();
            supported.Add("simple");
            supported.Add("complex");

            input = Console.ReadLine();
            while(!input.Equals("") && !supported.Contains(input))
            {
                Console.WriteLine("Shader not currently supported!");
                Console.WriteLine("Please enter your preferred shader, or leave blank to use default. Default: [" + Extensions.Shader + "]");
                Console.WriteLine("Supported: simple, complex");
                input = Console.ReadLine();
            }
            if (!input.Equals(""))
                Extensions.Shader = input;

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
            input = Console.ReadLine();
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            if (!input.Equals(""))
                Extensions.Normal = input;

            Console.WriteLine("Please enter your rough extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Rough + "]");
            input = Console.ReadLine();
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }            
            if (!input.Equals(""))
                Extensions.Rough = input;

            Console.WriteLine("Please enter your metalness extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Metal + "]");
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            input = Console.ReadLine();
            if (!input.Equals(""))
                Extensions.Metal = input;

            Console.WriteLine("Please enter your ambient occlusion extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Ambient + "]");
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            input = Console.ReadLine();
            if (!input.Equals(""))
                Extensions.Ambient = input;

            
            Console.WriteLine("Please enter your height extension if any (using wildcards), or leave blank to use default. Default: [" + Extensions.Height + "]");
            input = Console.ReadLine();
            while (input.Length > 0 && !input.Contains("*"))
            {
                Console.WriteLine("Please enter an extension containing a wildcard, or leave blank to use the default.");
                input = Console.ReadLine();
            }
            if (!input.Equals(""))
                Extensions.Height = input;

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
                string ambient = ""; //materials/default/default_ao.tga
                string metal = ""; //materials/default/default_metal.tga
                string height = "materials/default/default_height.tga";

                string[] normal_search = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*",""), Extensions.Normal.Replace("*", "")));
                if (normal_search.Length > 0)
                    normal = new_path + Path.GetFileName(normal_search[0]);
                string[] rough_search  = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*", ""), Extensions.Rough.Replace("*", "")));
                if(rough_search.Length > 0)    
                    rough = new_path + Path.GetFileName(rough_search[0]);
                string[] metal_search = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*", ""), Extensions.Metal.Replace("*", "")));
                if (metal_search.Length > 0) 
                    metal = new_path + Path.GetFileName(metal_search[0]);
                string[] ambient_search = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*", ""), Extensions.Ambient.Replace("*", "")));
                if (ambient_search.Length > 0)
                    ambient = new_path + Path.GetFileName(ambient_search[0]);
                
                
                //Special case shader overwrite for parallax (height maps)                
                string[] height_search = Directory.GetFiles(path, file_name.Replace(Extensions.Color.Replace("*", ""), Extensions.Height.Replace("*", "")));
                if (height_search.Length > 0)
                {
                    height = new_path + Path.GetFileName(height_search[0]);
                    MakeVMFParallax(base_path + "\\generated_vmat\\", file_name.Replace(Extensions.Color.Replace("*", ""), "").Replace(Extensions.FileType, "") + ".vmat", color, normal, rough, metal, ambient, height);
                    continue;
                }
                    

                switch (Extensions.Shader)
                {
                    case "simple":
                        MakeVMFSimple(base_path + "\\generated_vmat\\", file_name.Replace(Extensions.Color.Replace("*", ""), "").Replace(Extensions.FileType, "") + ".vmat", color, normal, rough, metal, ambient);
                        break;
                    case "complex":
                        MakeVMFComplex(base_path + "\\generated_vmat\\", file_name.Replace(Extensions.Color.Replace("*", ""), "").Replace(Extensions.FileType, "") + ".vmat", color, normal, rough, metal, ambient);
                        break;
                }
                    
                
            }
        }

        public static void MakeVMFSimple(string path, string file_name, string color, string normal, string rough, string metal, string ambient)
        {
            string template = "//THIS FILE WAS GENERATED BY TRIST'S BATCH TOOL \n" +
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
            "	TextureNormal \"" + normal + "\"\n" +
            "	TextureRoughness \"" + rough + "\"\n" +
            "	g_nScaleTexCoordUByModelScaleAxis \"0\"\n" +
            "	g_nScaleTexCoordVByModelScaleAxis \"0\"\n" +
            "	g_vTexCoordOffset \"[0.000 0.000]\"\n" +
            "	g_vTexCoordScale \"[1.000 1.000]\"\n" +
            "	g_vTexCoordScrollSpeed \"[0.000 0.000]\"\n";

            //PBR
            if (!metal.Equals(""))
                template += "	F_METALNESS_TEXTURE 1\n" +
                            "	TextureMetalness " + metal + "\n";
            else
                template += "	g_flMetalness \"0.000\"\n";

            if (!ambient.Equals(""))
                template += "	F_AMBIENT_OCCLUSION_TEXTURE 1\n" +
                    "	TextureAmbientOcclusion " + ambient + "\n";

            if (!metal.Equals("") && ambient.Equals(""))
                template += "	TextureAmbientOcclusion \"materials/default/default_ao.tga\"\n";

            template += "}";

            File.WriteAllText(path + file_name, template);
        }
        public static void MakeVMFComplex(string path, string file_name, string color, string normal, string rough, string metal, string ambient)
        {
            string template = "//THIS FILE WAS GENERATED BY TRIST'S BATCH TOOL \n" +
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
            "	g_vTexCoordScrollSpeed \"[0.000 0.000]\"\n";

            //PBR
            if (!metal.Equals(""))
                template += "	F_METALNESS_TEXTURE 1\n" +
                            "	TextureMetalness " + metal + "\n";
            else
                template += "	g_flMetalness \"0.000\"\n";

            if (!ambient.Equals(""))
                template += "	F_AMBIENT_OCCLUSION_TEXTURE 1\n" +
                    "	TextureAmbientOcclusion " + ambient + "\n";

            if (!metal.Equals("") && ambient.Equals(""))
                template += "	TextureAmbientOcclusion \"materials/default/default_ao.tga\"\n";

            template += "}";
            File.WriteAllText(path + file_name, template);
        }
        public static void MakeVMFParallax(string path, string file_name, string color, string normal, string rough, string metal, string ambient, string height)
        {
            string template = "//THIS FILE WAS GENERATED BY TRIST'S BATCH TOOL \n" +
            "Layer0\n" +
            "{\n" +
            "	shader \"vr_parallax_occlusion.vfx\"\n" +
            "   g_flModelTintAmount \"1.000\"\n" +
            "	g_vColorTint \"[1.000000 1.000000 1.000000 0.000000]\"\n" +
            "	TextureColor " + color + "\n" +
            "   g_flFadeExponent \"1.000\"\n" +
            "   g_bFogEnabled \"1\"\n" +
            "   g_flDirectionalLightmapMinZ \"0.050\"\n" +
            "	g_flDirectionalLightmapStrength \"1.000\"\n" +
            "   TextureNormal " + normal + "\n" +
            "   g_flHeightMapScale \"0.020\"\n" +
            "	g_nLODThreshold \"4\"\n" +
            "	g_nMaxSamples \"32\"\n" +
            "	g_nMinSamples \"8\"\n" +
            "	TextureHeight " + height + "\n" +
            "   TextureRoughness " + rough + "\n" +
            "   g_nScaleTexCoordUByModelScaleAxis \"0\"\n" +
            "	g_nScaleTexCoordVByModelScaleAxis \"0\"\n" +
            "	g_vTexCoordOffset \"[0.000 0.000]\"\n" +
            "	g_vTexCoordScale \"[1.000 1.000]\"\n" +
            "   g_vTexCoordScrollSpeed \"[0.000 0.000]\"\n";            

            //PBR
            if (!metal.Equals(""))
                template += "	TextureMetalness " + metal + "\n";
            else
                template += "   TextureMetalness \"materials/default/default_metal.tga\"\n";

            if (!ambient.Equals(""))
                template += "	TextureAmbientOcclusion " + ambient + "\n";
            else
                template += "	TextureAmbientOcclusion \"materials/default/default_ao.tga\"\n";

            template += "}";
            File.WriteAllText(path + file_name, template);
        }
    }
}

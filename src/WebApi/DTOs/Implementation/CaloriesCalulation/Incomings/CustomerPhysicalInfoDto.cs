using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.CaloriesCalulation.Incomings
{
    public enum ExeciseIntensity
    {
        /// <summary>
        ///     Do very least or not do exercise.
        /// </summary>
        VeryLow = 1,

        /// <summary>
        ///     Do exercise with rate around 1-3 times per week.
        /// </summary>
        Low,

        /// <summary>
        ///     Do exercise with rate around 4-5 times per week.
        /// </summary>
        Medium,

        /// <summary>
        ///     Do exercise with rate around 5-7 times per week.
        /// </summary>
        High,

        /// <summary>
        ///     Do exercise everyday.
        /// </summary>
        Energetic,

        /// <summary>
        ///     Do exercies everyday and do extreme work.
        /// </summary>
        Extreme
    }

    public static class ExerciseIntensityHelper
    {
        public static double GetIntensityCoefficient(ExeciseIntensity intensity)
        {
            switch (intensity)
            {
                case ExeciseIntensity.VeryLow:
                    return 1.2;

                case ExeciseIntensity.Low:
                    return 1.375;

                case ExeciseIntensity.Medium:
                    return 1.55;

                case ExeciseIntensity.High:
                    return 1.75;

                case ExeciseIntensity.Energetic:
                case ExeciseIntensity.Extreme:
                    return 1.9;

                default:
                    return 1.2;
            }
        }
    }

    public class CustomerPhysicalInfoDto
    {
        public const int MaleCoefficient = 5;
        public const int FemaleCoefficient = -161;

        public const int MinHeightInCentimeter = 100; // 1 meter.
        public const int MaxHeightInCentimeter = 300; // 3 meters.

        public const int MinWeightInKg = 30;
        public const int MaxWeightInKg = 200;

        public bool Gender { get; set; }

        [Range(minimum: MinHeightInCentimeter, maximum: MaxHeightInCentimeter)]
        public int HeightInCentimeter { get; set; }

        [Range(minimum: MinWeightInKg, maximum: MaxWeightInKg)]
        public int WeightInKg { get; set; }

        [Range(minimum: 1, maximum: 100)]
        public int Age { get; set; }

        public ExeciseIntensity Intensity { get; set; }

        /// <summary>
        ///     The calculation formula is referenced from this following website. <br/>
        ///     Link: <see cref="https://sieutinh.com/tinh-luong-calo-can-thiet-trong-ngay"/> 
        /// </summary>
        /// <returns></returns>
        public int CalculateStandardCaloriesNeed()
        {
            var coefficient = Gender ? MaleCoefficient : FemaleCoefficient;

            return (int) Math.Ceiling(6.25 * HeightInCentimeter + 10 * WeightInKg - 5 * Age + coefficient);
        }

        public int CalculateRecommendedCaloriesNeed()
        {
            var coefficient = Gender ? MaleCoefficient : FemaleCoefficient;

            var standardCaloriesNeed = CalculateStandardCaloriesNeed();

            var recommendedCaloriesNeed = 
                ExerciseIntensityHelper.GetIntensityCoefficient(Intensity) 
                * standardCaloriesNeed;

            return (int) Math.Ceiling(recommendedCaloriesNeed);
        }
    }
}

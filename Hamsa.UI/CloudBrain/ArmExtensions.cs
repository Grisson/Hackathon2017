﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Hamsa.REST
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Rest;
    using Models;

    /// <summary>
    /// Extension methods for Arm.
    /// </summary>
    public static partial class ArmExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static long? Register(this IArm operations)
            {
                return Task.Factory.StartNew(s => ((IArm)s).RegisterAsync(), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<long?> RegisterAsync(this IArm operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RegisterWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='timeStamp'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            public static bool? ReportPose(this IArm operations, long id, string timeStamp, int x, int y, int z)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ReportPoseAsync(id, timeStamp, x, y, z), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='timeStamp'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> ReportPoseAsync(this IArm operations, long id, string timeStamp, int x, int y, int z, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReportPoseWithHttpMessagesAsync(id, timeStamp, x, y, z, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='timeStamp'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            public static bool? ReportTouch(this IArm operations, long id, string timeStamp, double x, double y)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ReportTouchAsync(id, timeStamp, x, y), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='timeStamp'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> ReportTouchAsync(this IArm operations, long id, string timeStamp, double x, double y, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReportTouchWithHttpMessagesAsync(id, timeStamp, x, y, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static string CanResume(this IArm operations, long id)
            {
                return Task.Factory.StartNew(s => ((IArm)s).CanResumeAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> CanResumeAsync(this IArm operations, long id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CanResumeWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='data'>
            /// </param>
            public static void Done(this IArm operations, long id, string data)
            {
                Task.Factory.StartNew(s => ((IArm)s).DoneAsync(id, data), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='data'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task DoneAsync(this IArm operations, long id, string data, CancellationToken cancellationToken = default(CancellationToken))
            {
                await operations.DoneWithHttpMessagesAsync(id, data, null, cancellationToken).ConfigureAwait(false);
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static bool? WaitProb(this IArm operations, long id)
            {
                return Task.Factory.StartNew(s => ((IArm)s).WaitProbAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> WaitProbAsync(this IArm operations, long id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.WaitProbWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static string StartCalibrate(this IArm operations, long id)
            {
                return Task.Factory.StartNew(s => ((IArm)s).StartCalibrateAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> StartCalibrateAsync(this IArm operations, long id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.StartCalibrateWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='retry'>
            /// </param>
            public static string Prob(this IArm operations, long id, int retry)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ProbAsync(id, retry), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='retry'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> ProbAsync(this IArm operations, long id, int retry, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ProbWithHttpMessagesAsync(id, retry, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='retry'>
            /// </param>
            public static string GetNextTask(this IArm operations, long id, int? retry = default(int?))
            {
                return Task.Factory.StartNew(s => ((IArm)s).GetNextTaskAsync(id, retry), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='retry'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> GetNextTaskAsync(this IArm operations, long id, int? retry = default(int?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetNextTaskWithHttpMessagesAsync(id, retry, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            public static object ConvertCoordinateToPose(this IArm operations, long id, double x, double y, double z)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ConvertCoordinateToPoseAsync(id, x, y, z), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> ConvertCoordinateToPoseAsync(this IArm operations, long id, double x, double y, double z, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ConvertCoordinateToPoseWithHttpMessagesAsync(id, x, y, z, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            public static object ConvertPoseToCoordinate(this IArm operations, long id, int x, int y, int z)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ConvertPoseToCoordinateAsync(id, x, y, z), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='z'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> ConvertPoseToCoordinateAsync(this IArm operations, long id, int x, int y, int z, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ConvertPoseToCoordinateWithHttpMessagesAsync(id, x, y, z, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            public static object ConvertTouchPointToPose(this IArm operations, long id, double x, double y)
            {
                return Task.Factory.StartNew(s => ((IArm)s).ConvertTouchPointToPoseAsync(id, x, y), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='x'>
            /// </param>
            /// <param name='y'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> ConvertTouchPointToPoseAsync(this IArm operations, long id, double x, double y, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ConvertTouchPointToPoseWithHttpMessagesAsync(id, x, y, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='taskName'>
            /// </param>
            public static object NewTask(this IArm operations, long id, string taskName)
            {
                return Task.Factory.StartNew(s => ((IArm)s).NewTaskAsync(id, taskName), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='taskName'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> NewTaskAsync(this IArm operations, long id, string taskName, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.NewTaskWithHttpMessagesAsync(id, taskName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}

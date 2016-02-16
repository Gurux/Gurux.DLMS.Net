
namespace Gurux.DLMS.Enums
{
    /// <summary>
    ///  Enumerated DLMS error codes.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Server is close connection.
        /// </summary>
        DisconnectRequest = -3,
        /// <summary>
        /// Connection is rejected.
        /// </summary>
        Rejected = -2,
        /// <summary>
        /// Not a reply.
        /// </summary>
        InvalidHdlcReply = -1,

        /// <summary>
        /// No error has occurred.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Access Error : Device reports a hardware fault.
        /// </summary>
        HardwareFault = 1,

        /// <summary>
        /// Access Error : Device reports a temporary failure.
        /// </summary>
        TemporaryFailure = 2,

        /// <summary>
        /// Access Error : Device reports Read-Write denied.
        /// </summary>
        ReadWriteDenied = 3,

        /// <summary>
        /// Access Error : Device reports a undefined object
        /// </summary>
        UndefinedObject = 4,

        /// <summary>
        /// Access Error : Device reports a inconsistent Class or object
        /// </summary>
        InconsistentClass = 9,

        /// <summary>
        /// Access Error : Device reports a unavailable object
        /// </summary>
        UnavailableObject = 11,

        /// <summary>
        /// Access Error : Device reports a unmatched type
        /// </summary>
        UnmatchedType = 12,

        /// <summary>
        /// Access Error : Device reports scope of access violated
        /// </summary>
        AccessViolated = 13,

        /// <summary>
        /// Access Error : Data Block Unavailable.
        /// </summary>
        DataBlockUnavailable = 14,

        /// <summary>
        /// Access Error : Long Get Or Read Aborted.
        /// </summary>
        LongGetOrReadAborted = 15,

        /// <summary>
        /// Access Error : No Long Get Or Read In Progress.
        /// </summary>
        NoLongGetOrReadInProgress = 16,

        /// <summary>
        /// Access Error : Long Set Or Write Aborted.
        /// </summary>
        LongSetOrWriteAborted = 17,

        /// <summary>
        /// Access Error : No Long Set Or Write In Progress.
        /// </summary>
        NoLongSetOrWriteInProgress = 18,

        /// <summary>
        /// Access Error : Data Block Number Invalid.
        /// </summary>
        DataBlockNumberInvalid = 19,

        /// <summary>
        /// Access Error : Other Reason.
        /// </summary>
        OtherReason = 250
    }
}

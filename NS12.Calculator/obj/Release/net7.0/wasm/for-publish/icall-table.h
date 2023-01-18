#define ICALL_TABLE_corlib 1

static int corlib_icall_indexes [] = {
205,
210,
211,
212,
213,
214,
215,
216,
218,
219,
268,
269,
271,
294,
295,
296,
306,
307,
308,
309,
382,
383,
384,
387,
417,
418,
420,
422,
424,
426,
431,
439,
440,
441,
442,
443,
444,
445,
446,
447,
552,
560,
563,
565,
570,
571,
573,
574,
578,
579,
581,
582,
585,
586,
587,
590,
593,
595,
597,
658,
660,
662,
671,
672,
673,
675,
681,
682,
683,
684,
685,
693,
694,
695,
699,
700,
702,
704,
890,
1031,
1032,
5113,
5114,
5116,
5117,
5118,
5119,
5120,
5122,
5124,
5126,
5127,
5133,
5135,
5139,
5140,
5142,
5144,
5146,
5157,
5166,
5167,
5169,
5170,
5171,
5172,
5173,
5175,
5177,
6029,
6033,
6035,
6036,
6037,
6038,
6143,
6144,
6145,
6146,
6164,
6165,
6166,
6168,
6209,
6258,
6269,
6270,
6271,
6555,
6557,
6558,
6584,
6602,
6608,
6615,
6625,
6628,
6702,
6712,
6714,
6715,
6721,
6734,
6754,
6755,
6763,
6765,
6772,
6773,
6776,
6778,
6783,
6789,
6790,
6797,
6799,
6811,
6814,
6815,
6816,
6827,
6836,
6842,
6843,
6844,
6846,
6847,
6865,
6867,
6881,
6904,
6905,
6925,
6949,
6950,
7309,
7310,
7441,
7616,
7617,
7620,
7623,
7673,
7990,
7991,
8818,
8839,
8846,
8848,
};
void ves_icall_System_Array_InternalCreate (int,int,int,int,int);
int ves_icall_System_Array_GetCorElementTypeOfElementType_raw (int,int);
int ves_icall_System_Array_CanChangePrimitive (int,int,int);
int ves_icall_System_Array_FastCopy_raw (int,int,int,int,int,int);
int ves_icall_System_Array_GetLength_raw (int,int,int);
int ves_icall_System_Array_GetLowerBound_raw (int,int,int);
void ves_icall_System_Array_GetGenericValue_icall (int,int,int);
int ves_icall_System_Array_GetValueImpl_raw (int,int,int);
void ves_icall_System_Array_SetValueImpl_raw (int,int,int,int);
void ves_icall_System_Array_SetValueRelaxedImpl_raw (int,int,int,int);
void ves_icall_System_Runtime_RuntimeImports_Memmove (int,int,int);
void ves_icall_System_Buffer_BulkMoveWithWriteBarrier (int,int,int,int);
void ves_icall_System_Runtime_RuntimeImports_ZeroMemory (int,int);
int ves_icall_System_Delegate_AllocDelegateLike_internal_raw (int,int);
int ves_icall_System_Delegate_CreateDelegate_internal_raw (int,int,int,int,int);
int ves_icall_System_Delegate_GetVirtualMethod_internal_raw (int,int);
int ves_icall_System_Enum_GetEnumValuesAndNames_raw (int,int,int,int);
void ves_icall_System_Enum_InternalBoxEnum_raw (int,int,int64_t,int);
int ves_icall_System_Enum_InternalGetCorElementType (int);
void ves_icall_System_Enum_InternalGetUnderlyingType_raw (int,int,int);
int ves_icall_System_Environment_get_ProcessorCount ();
int ves_icall_System_Environment_get_TickCount ();
int64_t ves_icall_System_Environment_get_TickCount64 ();
void ves_icall_System_Environment_FailFast_raw (int,int,int,int);
void ves_icall_System_GC_register_ephemeron_array_raw (int,int);
int ves_icall_System_GC_get_ephemeron_tombstone_raw (int);
void ves_icall_System_GC_SuppressFinalize_raw (int,int);
void ves_icall_System_GC_ReRegisterForFinalize_raw (int,int);
void ves_icall_System_GC_GetGCMemoryInfo (int,int,int,int,int,int);
int ves_icall_System_GC_AllocPinnedArray_raw (int,int,int);
int ves_icall_System_Object_MemberwiseClone_raw (int,int);
double ves_icall_System_Math_Ceiling (double);
double ves_icall_System_Math_Cos (double);
double ves_icall_System_Math_Floor (double);
double ves_icall_System_Math_Log10 (double);
double ves_icall_System_Math_Pow (double,double);
double ves_icall_System_Math_Sin (double);
double ves_icall_System_Math_Sqrt (double);
double ves_icall_System_Math_Tan (double);
double ves_icall_System_Math_ModF (double,int);
int ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw (int,int,int);
void ves_icall_RuntimeType_make_array_type_raw (int,int,int,int);
void ves_icall_RuntimeType_make_byref_type_raw (int,int,int);
void ves_icall_RuntimeType_make_pointer_type_raw (int,int,int);
void ves_icall_RuntimeType_MakeGenericType_raw (int,int,int,int);
int ves_icall_RuntimeType_GetMethodsByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetPropertiesByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetConstructors_native_raw (int,int,int);
int ves_icall_System_RuntimeType_CreateInstanceInternal_raw (int,int);
void ves_icall_RuntimeType_GetDeclaringMethod_raw (int,int,int);
void ves_icall_System_RuntimeType_getFullName_raw (int,int,int,int,int);
void ves_icall_RuntimeType_GetGenericArgumentsInternal_raw (int,int,int,int);
int ves_icall_RuntimeType_GetGenericParameterPosition (int);
int ves_icall_RuntimeType_GetEvents_native_raw (int,int,int,int);
int ves_icall_RuntimeType_GetFields_native_raw (int,int,int,int,int);
void ves_icall_RuntimeType_GetInterfaces_raw (int,int,int);
void ves_icall_RuntimeType_GetDeclaringType_raw (int,int,int);
void ves_icall_RuntimeType_GetName_raw (int,int,int);
void ves_icall_RuntimeType_GetNamespace_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_GetAttributes (int);
int ves_icall_RuntimeTypeHandle_GetMetadataToken_raw (int,int);
void ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_GetCorElementType (int);
int ves_icall_RuntimeTypeHandle_HasInstantiation (int);
int ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_HasReferences_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetArrayRank_raw (int,int);
void ves_icall_RuntimeTypeHandle_GetAssembly_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetElementType_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetModule_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetBaseType_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition (int);
int ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw (int,int);
int ves_icall_RuntimeTypeHandle_is_subclass_of_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_IsByRefLike_raw (int,int);
void ves_icall_System_RuntimeTypeHandle_internal_from_name_raw (int,int,int,int,int,int);
int ves_icall_System_String_FastAllocateString_raw (int,int);
int ves_icall_System_Type_internal_from_handle_raw (int,int);
int ves_icall_System_ValueType_InternalGetHashCode_raw (int,int,int);
int ves_icall_System_ValueType_Equals_raw (int,int,int,int);
int ves_icall_System_Threading_Interlocked_CompareExchange_Int (int,int,int);
void ves_icall_System_Threading_Interlocked_CompareExchange_Object (int,int,int,int);
int ves_icall_System_Threading_Interlocked_Decrement_Int (int);
int ves_icall_System_Threading_Interlocked_Increment_Int (int);
int64_t ves_icall_System_Threading_Interlocked_Increment_Long (int);
int ves_icall_System_Threading_Interlocked_Exchange_Int (int,int);
void ves_icall_System_Threading_Interlocked_Exchange_Object (int,int,int);
int64_t ves_icall_System_Threading_Interlocked_CompareExchange_Long (int,int64_t,int64_t);
int64_t ves_icall_System_Threading_Interlocked_Exchange_Long (int,int64_t);
int ves_icall_System_Threading_Interlocked_Add_Int (int,int);
int64_t ves_icall_System_Threading_Interlocked_Add_Long (int,int64_t);
void ves_icall_System_Threading_Monitor_Monitor_Enter_raw (int,int);
void mono_monitor_exit_icall_raw (int,int);
int ves_icall_System_Threading_Monitor_Monitor_test_synchronised_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw (int,int);
int ves_icall_System_Threading_Monitor_Monitor_wait_raw (int,int,int,int);
void ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw (int,int,int,int,int);
int ves_icall_System_Threading_Thread_GetCurrentProcessorNumber_raw (int);
void ves_icall_System_Threading_Thread_InitInternal_raw (int,int);
int ves_icall_System_Threading_Thread_GetCurrentThread ();
void ves_icall_System_Threading_InternalThread_Thread_free_internal_raw (int,int);
int ves_icall_System_Threading_Thread_GetState_raw (int,int);
void ves_icall_System_Threading_Thread_SetState_raw (int,int,int);
void ves_icall_System_Threading_Thread_ClrState_raw (int,int,int);
void ves_icall_System_Threading_Thread_SetName_icall_raw (int,int,int,int);
int ves_icall_System_Threading_Thread_YieldInternal ();
void ves_icall_System_Threading_Thread_SetPriority_raw (int,int,int);
void ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw (int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw (int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw (int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw (int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw (int,int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw (int);
int ves_icall_System_GCHandle_InternalAlloc_raw (int,int,int);
void ves_icall_System_GCHandle_InternalFree_raw (int,int);
int ves_icall_System_GCHandle_InternalGet_raw (int,int);
void ves_icall_System_GCHandle_InternalSet_raw (int,int,int);
int ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError ();
void ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError (int);
void ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw (int,int,int,int);
int ves_icall_System_Runtime_InteropServices_Marshal_SizeOfHelper_raw (int,int,int);
int ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw (int,int,int,int,int,int);
int mono_object_hash_icall_raw (int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw (int,int);
void ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw (int,int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack ();
int ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw (int);
int ves_icall_System_Reflection_Assembly_InternalLoad_raw (int,int,int,int);
int ves_icall_System_Reflection_Assembly_InternalGetType_raw (int,int,int,int,int,int);
int ves_icall_System_Reflection_AssemblyName_GetNativeName (int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw (int,int,int,int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw (int,int);
int ves_icall_MonoCustomAttrs_IsDefinedInternal_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw (int,int);
void ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeAssembly_GetInfo_raw (int,int,int,int);
void ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeAssembly_GetModulesInternal_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw (int,int,int,int,int,int,int);
void ves_icall_RuntimeEventInfo_get_event_info_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_ResolveType_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetParentType_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_GetFieldOffset_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetValueInternal_raw (int,int,int);
void ves_icall_RuntimeFieldInfo_SetValueInternal_raw (int,int,int,int);
int ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw (int,int);
int ves_icall_reflection_get_token_raw (int,int);
void ves_icall_get_method_info_raw (int,int,int);
int ves_icall_get_method_attributes (int);
int ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw (int,int,int);
int ves_icall_System_MonoMethodInfo_get_retval_marshal_raw (int,int);
int ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw (int,int,int,int);
int ves_icall_RuntimeMethodInfo_get_name_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_base_method_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
void ves_icall_RuntimeMethodInfo_GetPInvoke_raw (int,int,int,int,int);
int ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw (int,int,int);
int ves_icall_RuntimeMethodInfo_GetGenericArguments_raw (int,int);
int ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw (int,int);
void ves_icall_InvokeClassConstructor_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
void ves_icall_System_Reflection_RuntimeModule_GetGuidInternal_raw (int,int,int);
int ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw (int,int,int,int,int,int);
void ves_icall_RuntimePropertyInfo_get_property_info_raw (int,int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw (int,int,int);
void ves_icall_AssemblyExtensions_ApplyUpdate (int,int,int,int,int,int,int);
void ves_icall_AssemblyBuilder_basic_init_raw (int,int);
void ves_icall_DynamicMethod_create_dynamic_method_raw (int,int);
void ves_icall_ModuleBuilder_basic_init_raw (int,int);
void ves_icall_ModuleBuilder_set_wrappers_type_raw (int,int,int);
int ves_icall_ModuleBuilder_getToken_raw (int,int,int,int);
void ves_icall_ModuleBuilder_RegisterToken_raw (int,int,int,int);
int ves_icall_TypeBuilder_create_runtime_class_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw (int,int);
int ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass (int);
void ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree (int);
int ves_icall_Mono_SafeStringMarshal_StringToUtf8 (int);
void ves_icall_Mono_SafeStringMarshal_GFree (int);
static void *corlib_icall_funcs [] = {
// token 205,
ves_icall_System_Array_InternalCreate,
// token 210,
ves_icall_System_Array_GetCorElementTypeOfElementType_raw,
// token 211,
ves_icall_System_Array_CanChangePrimitive,
// token 212,
ves_icall_System_Array_FastCopy_raw,
// token 213,
ves_icall_System_Array_GetLength_raw,
// token 214,
ves_icall_System_Array_GetLowerBound_raw,
// token 215,
ves_icall_System_Array_GetGenericValue_icall,
// token 216,
ves_icall_System_Array_GetValueImpl_raw,
// token 218,
ves_icall_System_Array_SetValueImpl_raw,
// token 219,
ves_icall_System_Array_SetValueRelaxedImpl_raw,
// token 268,
ves_icall_System_Runtime_RuntimeImports_Memmove,
// token 269,
ves_icall_System_Buffer_BulkMoveWithWriteBarrier,
// token 271,
ves_icall_System_Runtime_RuntimeImports_ZeroMemory,
// token 294,
ves_icall_System_Delegate_AllocDelegateLike_internal_raw,
// token 295,
ves_icall_System_Delegate_CreateDelegate_internal_raw,
// token 296,
ves_icall_System_Delegate_GetVirtualMethod_internal_raw,
// token 306,
ves_icall_System_Enum_GetEnumValuesAndNames_raw,
// token 307,
ves_icall_System_Enum_InternalBoxEnum_raw,
// token 308,
ves_icall_System_Enum_InternalGetCorElementType,
// token 309,
ves_icall_System_Enum_InternalGetUnderlyingType_raw,
// token 382,
ves_icall_System_Environment_get_ProcessorCount,
// token 383,
ves_icall_System_Environment_get_TickCount,
// token 384,
ves_icall_System_Environment_get_TickCount64,
// token 387,
ves_icall_System_Environment_FailFast_raw,
// token 417,
ves_icall_System_GC_register_ephemeron_array_raw,
// token 418,
ves_icall_System_GC_get_ephemeron_tombstone_raw,
// token 420,
ves_icall_System_GC_SuppressFinalize_raw,
// token 422,
ves_icall_System_GC_ReRegisterForFinalize_raw,
// token 424,
ves_icall_System_GC_GetGCMemoryInfo,
// token 426,
ves_icall_System_GC_AllocPinnedArray_raw,
// token 431,
ves_icall_System_Object_MemberwiseClone_raw,
// token 439,
ves_icall_System_Math_Ceiling,
// token 440,
ves_icall_System_Math_Cos,
// token 441,
ves_icall_System_Math_Floor,
// token 442,
ves_icall_System_Math_Log10,
// token 443,
ves_icall_System_Math_Pow,
// token 444,
ves_icall_System_Math_Sin,
// token 445,
ves_icall_System_Math_Sqrt,
// token 446,
ves_icall_System_Math_Tan,
// token 447,
ves_icall_System_Math_ModF,
// token 552,
ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw,
// token 560,
ves_icall_RuntimeType_make_array_type_raw,
// token 563,
ves_icall_RuntimeType_make_byref_type_raw,
// token 565,
ves_icall_RuntimeType_make_pointer_type_raw,
// token 570,
ves_icall_RuntimeType_MakeGenericType_raw,
// token 571,
ves_icall_RuntimeType_GetMethodsByName_native_raw,
// token 573,
ves_icall_RuntimeType_GetPropertiesByName_native_raw,
// token 574,
ves_icall_RuntimeType_GetConstructors_native_raw,
// token 578,
ves_icall_System_RuntimeType_CreateInstanceInternal_raw,
// token 579,
ves_icall_RuntimeType_GetDeclaringMethod_raw,
// token 581,
ves_icall_System_RuntimeType_getFullName_raw,
// token 582,
ves_icall_RuntimeType_GetGenericArgumentsInternal_raw,
// token 585,
ves_icall_RuntimeType_GetGenericParameterPosition,
// token 586,
ves_icall_RuntimeType_GetEvents_native_raw,
// token 587,
ves_icall_RuntimeType_GetFields_native_raw,
// token 590,
ves_icall_RuntimeType_GetInterfaces_raw,
// token 593,
ves_icall_RuntimeType_GetDeclaringType_raw,
// token 595,
ves_icall_RuntimeType_GetName_raw,
// token 597,
ves_icall_RuntimeType_GetNamespace_raw,
// token 658,
ves_icall_RuntimeTypeHandle_GetAttributes,
// token 660,
ves_icall_RuntimeTypeHandle_GetMetadataToken_raw,
// token 662,
ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw,
// token 671,
ves_icall_RuntimeTypeHandle_GetCorElementType,
// token 672,
ves_icall_RuntimeTypeHandle_HasInstantiation,
// token 673,
ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw,
// token 675,
ves_icall_RuntimeTypeHandle_HasReferences_raw,
// token 681,
ves_icall_RuntimeTypeHandle_GetArrayRank_raw,
// token 682,
ves_icall_RuntimeTypeHandle_GetAssembly_raw,
// token 683,
ves_icall_RuntimeTypeHandle_GetElementType_raw,
// token 684,
ves_icall_RuntimeTypeHandle_GetModule_raw,
// token 685,
ves_icall_RuntimeTypeHandle_GetBaseType_raw,
// token 693,
ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw,
// token 694,
ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition,
// token 695,
ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw,
// token 699,
ves_icall_RuntimeTypeHandle_is_subclass_of_raw,
// token 700,
ves_icall_RuntimeTypeHandle_IsByRefLike_raw,
// token 702,
ves_icall_System_RuntimeTypeHandle_internal_from_name_raw,
// token 704,
ves_icall_System_String_FastAllocateString_raw,
// token 890,
ves_icall_System_Type_internal_from_handle_raw,
// token 1031,
ves_icall_System_ValueType_InternalGetHashCode_raw,
// token 1032,
ves_icall_System_ValueType_Equals_raw,
// token 5113,
ves_icall_System_Threading_Interlocked_CompareExchange_Int,
// token 5114,
ves_icall_System_Threading_Interlocked_CompareExchange_Object,
// token 5116,
ves_icall_System_Threading_Interlocked_Decrement_Int,
// token 5117,
ves_icall_System_Threading_Interlocked_Increment_Int,
// token 5118,
ves_icall_System_Threading_Interlocked_Increment_Long,
// token 5119,
ves_icall_System_Threading_Interlocked_Exchange_Int,
// token 5120,
ves_icall_System_Threading_Interlocked_Exchange_Object,
// token 5122,
ves_icall_System_Threading_Interlocked_CompareExchange_Long,
// token 5124,
ves_icall_System_Threading_Interlocked_Exchange_Long,
// token 5126,
ves_icall_System_Threading_Interlocked_Add_Int,
// token 5127,
ves_icall_System_Threading_Interlocked_Add_Long,
// token 5133,
ves_icall_System_Threading_Monitor_Monitor_Enter_raw,
// token 5135,
mono_monitor_exit_icall_raw,
// token 5139,
ves_icall_System_Threading_Monitor_Monitor_test_synchronised_raw,
// token 5140,
ves_icall_System_Threading_Monitor_Monitor_pulse_raw,
// token 5142,
ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw,
// token 5144,
ves_icall_System_Threading_Monitor_Monitor_wait_raw,
// token 5146,
ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw,
// token 5157,
ves_icall_System_Threading_Thread_GetCurrentProcessorNumber_raw,
// token 5166,
ves_icall_System_Threading_Thread_InitInternal_raw,
// token 5167,
ves_icall_System_Threading_Thread_GetCurrentThread,
// token 5169,
ves_icall_System_Threading_InternalThread_Thread_free_internal_raw,
// token 5170,
ves_icall_System_Threading_Thread_GetState_raw,
// token 5171,
ves_icall_System_Threading_Thread_SetState_raw,
// token 5172,
ves_icall_System_Threading_Thread_ClrState_raw,
// token 5173,
ves_icall_System_Threading_Thread_SetName_icall_raw,
// token 5175,
ves_icall_System_Threading_Thread_YieldInternal,
// token 5177,
ves_icall_System_Threading_Thread_SetPriority_raw,
// token 6029,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw,
// token 6033,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw,
// token 6035,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw,
// token 6036,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw,
// token 6037,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw,
// token 6038,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw,
// token 6143,
ves_icall_System_GCHandle_InternalAlloc_raw,
// token 6144,
ves_icall_System_GCHandle_InternalFree_raw,
// token 6145,
ves_icall_System_GCHandle_InternalGet_raw,
// token 6146,
ves_icall_System_GCHandle_InternalSet_raw,
// token 6164,
ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError,
// token 6165,
ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError,
// token 6166,
ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw,
// token 6168,
ves_icall_System_Runtime_InteropServices_Marshal_SizeOfHelper_raw,
// token 6209,
ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw,
// token 6258,
mono_object_hash_icall_raw,
// token 6269,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw,
// token 6270,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw,
// token 6271,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack,
// token 6555,
ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw,
// token 6557,
ves_icall_System_Reflection_Assembly_InternalLoad_raw,
// token 6558,
ves_icall_System_Reflection_Assembly_InternalGetType_raw,
// token 6584,
ves_icall_System_Reflection_AssemblyName_GetNativeName,
// token 6602,
ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw,
// token 6608,
ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw,
// token 6615,
ves_icall_MonoCustomAttrs_IsDefinedInternal_raw,
// token 6625,
ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw,
// token 6628,
ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw,
// token 6702,
ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw,
// token 6712,
ves_icall_System_Reflection_RuntimeAssembly_GetInfo_raw,
// token 6714,
ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw,
// token 6715,
ves_icall_System_Reflection_RuntimeAssembly_GetModulesInternal_raw,
// token 6721,
ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw,
// token 6734,
ves_icall_RuntimeEventInfo_get_event_info_raw,
// token 6754,
ves_icall_reflection_get_token_raw,
// token 6755,
ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw,
// token 6763,
ves_icall_RuntimeFieldInfo_ResolveType_raw,
// token 6765,
ves_icall_RuntimeFieldInfo_GetParentType_raw,
// token 6772,
ves_icall_RuntimeFieldInfo_GetFieldOffset_raw,
// token 6773,
ves_icall_RuntimeFieldInfo_GetValueInternal_raw,
// token 6776,
ves_icall_RuntimeFieldInfo_SetValueInternal_raw,
// token 6778,
ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw,
// token 6783,
ves_icall_reflection_get_token_raw,
// token 6789,
ves_icall_get_method_info_raw,
// token 6790,
ves_icall_get_method_attributes,
// token 6797,
ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw,
// token 6799,
ves_icall_System_MonoMethodInfo_get_retval_marshal_raw,
// token 6811,
ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw,
// token 6814,
ves_icall_RuntimeMethodInfo_get_name_raw,
// token 6815,
ves_icall_RuntimeMethodInfo_get_base_method_raw,
// token 6816,
ves_icall_reflection_get_token_raw,
// token 6827,
ves_icall_InternalInvoke_raw,
// token 6836,
ves_icall_RuntimeMethodInfo_GetPInvoke_raw,
// token 6842,
ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw,
// token 6843,
ves_icall_RuntimeMethodInfo_GetGenericArguments_raw,
// token 6844,
ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw,
// token 6846,
ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw,
// token 6847,
ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw,
// token 6865,
ves_icall_InvokeClassConstructor_raw,
// token 6867,
ves_icall_InternalInvoke_raw,
// token 6881,
ves_icall_reflection_get_token_raw,
// token 6904,
ves_icall_System_Reflection_RuntimeModule_GetGuidInternal_raw,
// token 6905,
ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw,
// token 6925,
ves_icall_RuntimePropertyInfo_get_property_info_raw,
// token 6949,
ves_icall_reflection_get_token_raw,
// token 6950,
ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw,
// token 7309,
ves_icall_AssemblyExtensions_ApplyUpdate,
// token 7310,
ves_icall_AssemblyBuilder_basic_init_raw,
// token 7441,
ves_icall_DynamicMethod_create_dynamic_method_raw,
// token 7616,
ves_icall_ModuleBuilder_basic_init_raw,
// token 7617,
ves_icall_ModuleBuilder_set_wrappers_type_raw,
// token 7620,
ves_icall_ModuleBuilder_getToken_raw,
// token 7623,
ves_icall_ModuleBuilder_RegisterToken_raw,
// token 7673,
ves_icall_TypeBuilder_create_runtime_class_raw,
// token 7990,
ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw,
// token 7991,
ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw,
// token 8818,
ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass,
// token 8839,
ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree,
// token 8846,
ves_icall_Mono_SafeStringMarshal_StringToUtf8,
// token 8848,
ves_icall_Mono_SafeStringMarshal_GFree,
};
static uint8_t corlib_icall_handles [] = {
0,
1,
0,
1,
1,
1,
0,
1,
1,
1,
0,
0,
0,
1,
1,
1,
1,
1,
0,
1,
0,
0,
0,
1,
1,
1,
1,
1,
0,
1,
1,
0,
0,
0,
0,
0,
0,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
0,
1,
1,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
0,
};

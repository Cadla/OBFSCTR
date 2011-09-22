﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Obfuscator.Test.Overrides
{
    internal enum TypeIcon
    {
        Class,
        Enum,
        Struct,
        Interface,
        Delegate
    }

    internal enum MemberIcon
    {
        Literal,
        FieldReadOnly,
        Field,
        EnumValue,
        Property,
        Indexer,
        Method,
        Constructor,
        VirtualMethod,
        Operator,
        ExtensionMethod,
        Event
    }

    internal enum AccessOverlayIcon
    {
        Public,
        Protected,
        Internal,
        ProtectedInternal,
        Private,
    }

    static class Images
    {
        static BitmapImage LoadBitmap(string name)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/" + name + ".png"));
            image.Freeze();
            return image;
        }

        public static readonly BitmapImage ViewCode = LoadBitmap("ViewCode");
        public static readonly BitmapImage Save = LoadBitmap("SaveFile");
        public static readonly BitmapImage OK = LoadBitmap("OK");

        public static readonly BitmapImage Delete = LoadBitmap("Delete");
        public static readonly BitmapImage Search = LoadBitmap("Search");

        public static readonly BitmapImage Assembly = LoadBitmap("Assembly");
        public static readonly BitmapImage AssemblyWarning = LoadBitmap("AssemblyWarning");
        public static readonly BitmapImage AssemblyLoading = LoadBitmap("FindAssembly");

        public static readonly BitmapImage Library = LoadBitmap("Library");
        public static readonly BitmapImage Namespace = LoadBitmap("NameSpace");

        public static readonly BitmapImage ReferenceFolderOpen = LoadBitmap("ReferenceFolder.Open");
        public static readonly BitmapImage ReferenceFolderClosed = LoadBitmap("ReferenceFolder.Closed");

        public static readonly BitmapImage SubTypes = LoadBitmap("SubTypes");
        public static readonly BitmapImage SuperTypes = LoadBitmap("SuperTypes");

        public static readonly BitmapImage FolderOpen = LoadBitmap("Folder.Open");
        public static readonly BitmapImage FolderClosed = LoadBitmap("Folder.Closed");

        public static readonly BitmapImage Resource = LoadBitmap("Resource");
        public static readonly BitmapImage ResourceImage = LoadBitmap("ResourceImage");
        public static readonly BitmapImage ResourceResourcesFile = LoadBitmap("ResourceResourcesFile");

        public static readonly BitmapImage Class = LoadBitmap("Class");
        public static readonly BitmapImage Struct = LoadBitmap("Struct");
        public static readonly BitmapImage Interface = LoadBitmap("Interface");
        public static readonly BitmapImage Delegate = LoadBitmap("Delegate");
        public static readonly BitmapImage Enum = LoadBitmap("Enum");


        private static readonly BitmapImage Field = LoadBitmap("Field");
        private static readonly BitmapImage FieldReadOnly = LoadBitmap("FieldReadOnly");
        private static readonly BitmapImage Literal = LoadBitmap("Literal");
        private static readonly BitmapImage EnumValue = LoadBitmap("EnumValue");

        private static readonly BitmapImage Method = LoadBitmap("Method");
        private static readonly BitmapImage Constructor = LoadBitmap("Constructor");
        private static readonly BitmapImage VirtualMethod = LoadBitmap("VirtualMethod");
        private static readonly BitmapImage Operator = LoadBitmap("Operator");
        private static readonly BitmapImage ExtensionMethod = LoadBitmap("ExtensionMethod");

        private static readonly BitmapImage Property = LoadBitmap("Property");
        private static readonly BitmapImage Indexer = LoadBitmap("Indexer");

        private static readonly BitmapImage Event = LoadBitmap("Event");

        private static readonly BitmapImage OverlayProtected = LoadBitmap("OverlayProtected");
        private static readonly BitmapImage OverlayInternal = LoadBitmap("OverlayInternal");
        private static readonly BitmapImage OverlayProtectedInternal = LoadBitmap("OverlayProtectedInternal");
        private static readonly BitmapImage OverlayPrivate = LoadBitmap("OverlayPrivate");

        private static readonly BitmapImage OverlayStatic = LoadBitmap("OverlayStatic");

        public static BitmapImage LoadImage(object part, string icon)
        {
            Uri uri;
            var assembly = part.GetType().Assembly;
            if (assembly == typeof(Images).Assembly)
            {
                uri = new Uri("pack://application:,,,/" + icon);
            }
            else
            {
                var name = assembly.GetName();
                uri = new Uri("pack://application:,,,/" + name.Name + ";v" + name.Version + ";component/" + icon);
            }
            BitmapImage image = new BitmapImage(uri);
            image.Freeze();
            return image;
        }


        private static TypeIconCache typeIconCache = new TypeIconCache();
        private static MemberIconCache memberIconCache = new MemberIconCache();

        public static ImageSource GetIcon(TypeIcon icon, AccessOverlayIcon overlay)
        {
            return typeIconCache.GetIcon(icon, overlay, false);
        }

        public static ImageSource GetIcon(MemberIcon icon, AccessOverlayIcon overlay, bool isStatic)
        {
            return memberIconCache.GetIcon(icon, overlay, isStatic);
        }

        #region icon caches & overlay management

        private class TypeIconCache : IconCache<TypeIcon>
        {
            public TypeIconCache()
            {
                PreloadPublicIconToCache(TypeIcon.Class, Images.Class);
                PreloadPublicIconToCache(TypeIcon.Enum, Images.Enum);
                PreloadPublicIconToCache(TypeIcon.Struct, Images.Struct);
                PreloadPublicIconToCache(TypeIcon.Interface, Images.Interface);
                PreloadPublicIconToCache(TypeIcon.Delegate, Images.Delegate);
            }

            protected override ImageSource GetBaseImage(TypeIcon icon)
            {
                ImageSource baseImage;
                switch (icon)
                {
                    case TypeIcon.Class:
                        baseImage = Images.Class;
                        break;
                    case TypeIcon.Enum:
                        baseImage = Images.Enum;
                        break;
                    case TypeIcon.Struct:
                        baseImage = Images.Struct;
                        break;
                    case TypeIcon.Interface:
                        baseImage = Images.Interface;
                        break;
                    case TypeIcon.Delegate:
                        baseImage = Images.Delegate;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return baseImage;
            }
        }

        private class MemberIconCache : IconCache<MemberIcon>
        {
            public MemberIconCache()
            {
                PreloadPublicIconToCache(MemberIcon.Field, Images.Field);
                PreloadPublicIconToCache(MemberIcon.FieldReadOnly, Images.FieldReadOnly);
                PreloadPublicIconToCache(MemberIcon.Literal, Images.Literal);
                PreloadPublicIconToCache(MemberIcon.EnumValue, Images.EnumValue);
                PreloadPublicIconToCache(MemberIcon.Property, Images.Property);
                PreloadPublicIconToCache(MemberIcon.Indexer, Images.Indexer);
                PreloadPublicIconToCache(MemberIcon.Method, Images.Method);
                PreloadPublicIconToCache(MemberIcon.Constructor, Images.Constructor);
                PreloadPublicIconToCache(MemberIcon.VirtualMethod, Images.VirtualMethod);
                PreloadPublicIconToCache(MemberIcon.Operator, Images.Operator);
                PreloadPublicIconToCache(MemberIcon.ExtensionMethod, Images.ExtensionMethod);
                PreloadPublicIconToCache(MemberIcon.Event, Images.Event);
            }

            protected override ImageSource GetBaseImage(MemberIcon icon)
            {
                ImageSource baseImage;
                switch (icon)
                {
                    case MemberIcon.Field:
                        baseImage = Images.Field;
                        break;
                    case MemberIcon.FieldReadOnly:
                        baseImage = Images.FieldReadOnly;
                        break;
                    case MemberIcon.Literal:
                        baseImage = Images.Literal;
                        break;
                    case MemberIcon.EnumValue:
                        baseImage = Images.Literal;
                        break;
                    case MemberIcon.Property:
                        baseImage = Images.Property;
                        break;
                    case MemberIcon.Indexer:
                        baseImage = Images.Indexer;
                        break;
                    case MemberIcon.Method:
                        baseImage = Images.Method;
                        break;
                    case MemberIcon.Constructor:
                        baseImage = Images.Constructor;
                        break;
                    case MemberIcon.VirtualMethod:
                        baseImage = Images.VirtualMethod;
                        break;
                    case MemberIcon.Operator:
                        baseImage = Images.Operator;
                        break;
                    case MemberIcon.ExtensionMethod:
                        baseImage = Images.ExtensionMethod;
                        break;
                    case MemberIcon.Event:
                        baseImage = Images.Event;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return baseImage;
            }
        }

        private abstract class IconCache<T>
        {
            private Dictionary<Tuple<T, AccessOverlayIcon, bool>, ImageSource> cache = new Dictionary<Tuple<T, AccessOverlayIcon, bool>, ImageSource>();

            protected void PreloadPublicIconToCache(T icon, ImageSource image)
            {
                var iconKey = new Tuple<T, AccessOverlayIcon, bool>(icon, AccessOverlayIcon.Public, false);
                cache.Add(iconKey, image);
            }

            public ImageSource GetIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
            {
                var iconKey = new Tuple<T, AccessOverlayIcon, bool>(icon, overlay, isStatic);
                if (cache.ContainsKey(iconKey))
                {
                    return cache[iconKey];
                }
                else
                {
                    ImageSource result = BuildMemberIcon(icon, overlay, isStatic);
                    cache.Add(iconKey, result);
                    return result;
                }
            }

            private ImageSource BuildMemberIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
            {
                ImageSource baseImage = GetBaseImage(icon);
                ImageSource overlayImage = GetOverlayImage(overlay);

                return CreateOverlayImage(baseImage, overlayImage, isStatic);
            }

            protected abstract ImageSource GetBaseImage(T icon);

            private static ImageSource GetOverlayImage(AccessOverlayIcon overlay)
            {
                ImageSource overlayImage;
                switch (overlay)
                {
                    case AccessOverlayIcon.Public:
                        overlayImage = null;
                        break;
                    case AccessOverlayIcon.Protected:
                        overlayImage = Images.OverlayProtected;
                        break;
                    case AccessOverlayIcon.Internal:
                        overlayImage = Images.OverlayInternal;
                        break;
                    case AccessOverlayIcon.ProtectedInternal:
                        overlayImage = Images.OverlayProtectedInternal;
                        break;
                    case AccessOverlayIcon.Private:
                        overlayImage = Images.OverlayPrivate;
                        break;
                    default:
                        throw new NotSupportedException();
                }
                return overlayImage;
            }

            private static readonly Rect iconRect = new Rect(0, 0, 16, 16);

            private static ImageSource CreateOverlayImage(ImageSource baseImage, ImageSource overlay, bool isStatic)
            {
                var group = new DrawingGroup();

                group.Children.Add(new ImageDrawing(baseImage, iconRect));

                if (overlay != null)
                {
                    group.Children.Add(new ImageDrawing(overlay, iconRect));
                }

                if (isStatic)
                {
                    group.Children.Add(new ImageDrawing(Images.OverlayStatic, iconRect));
                }

                var image = new DrawingImage(group);
                image.Freeze();
                return image;
            }
        }

        #endregion
    }
}

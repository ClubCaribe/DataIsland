module diapp.models.filemanager {
        
    export interface IDiDirectoryListingEntry {
        Name: string;
        FullName: string;
        Extension: string;
        Size: number;
        IsDirectory: boolean;
        FileSystemObject: any;
        IsShared: boolean;
        IsSharedAsPublic: boolean;
        SharedPublicPath: string;
        Checked?: boolean;
        isInRenameMode?: boolean;
        isMouseOver?: boolean;
        NewName?: string;
    }

    export interface IDiForeignResourcePermissions {
        Read: boolean;
        Write: boolean;
        All: boolean;
    }

    export interface IPlUploaderArgs {
        fileUploadUrl: string;
        maxFileSize: string;
        chunkSize: string;
        flashSwfUrl: string;
        silverlightXapUrl: string;
    }

    export interface IDirectorySummary {
        TotalSize: number;
        TotalNumFiles: number;
    }

    export interface IFileViewDetails {
        isDetailsViewExpanded: boolean;
        imageContainerWidth: number;
        imageContainerHeight: number;
    }

    
}
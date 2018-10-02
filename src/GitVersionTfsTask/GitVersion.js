"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const tl = require("vsts-task-lib/task");
const path = require("path");
const os = require("os");
var updateAssemblyInfo = tl.getBoolInput('updateAssemblyInfo');
var updateAssemblyInfoFilename = tl.getInput('updateAssemblyInfoFilename');
var additionalArguments = tl.getInput('additionalArguments');
var gitVersionPath = tl.getInput('gitVersionPath');
var preferBundledVersion = tl.getBoolInput('preferBundledVersion');
var currentDirectory = __dirname;
var sourcesDirectory = tl.getVariable("Build.SourcesDirectory");
if (!gitVersionPath) {
    gitVersionPath = tl.which("GitVersion.exe");
    if (preferBundledVersion || !gitVersionPath) {
        gitVersionPath = path.join(currentDirectory, "GitVersion.exe");
    }
}
(function execute() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            var execOptions = {
                cwd: undefined,
                env: undefined,
                silent: undefined,
                failOnStdErr: undefined,
                ignoreReturnCode: undefined,
                errStream: undefined,
                outStream: undefined,
                windowsVerbatimArguments: undefined
            };
            var toolRunner;
            var isWin32 = os.platform() == "win32";
            if (isWin32) {
                toolRunner = tl.tool(gitVersionPath);
            }
            else {
                toolRunner = tl.tool("mono");
                toolRunner.arg(gitVersionPath);
            }
            toolRunner.arg([
                sourcesDirectory,
                "/output",
                "buildserver",
                "/nofetch"
            ]);
            if (updateAssemblyInfo) {
                toolRunner.arg("/updateassemblyinfo");
                if (updateAssemblyInfoFilename) {
                    toolRunner.arg(updateAssemblyInfoFilename);
                }
                else {
                    toolRunner.arg("true");
                }
            }
            if (additionalArguments) {
                toolRunner.line(additionalArguments);
            }
            var result = yield toolRunner.exec(execOptions);
            if (result) {
                tl.setResult(tl.TaskResult.Failed, "An error occured during GitVersion execution");
            }
            else {
                tl.setResult(tl.TaskResult.Succeeded, "GitVersion executed successfully");
            }
        }
        catch (err) {
            tl.debug(err.stack);
            tl.setResult(tl.TaskResult.Failed, err);
        }
    });
})();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiR2l0VmVyc2lvbi5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIkdpdFZlcnNpb24udHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6Ijs7Ozs7Ozs7OztBQUFBLHlDQUEwQztBQUUxQyw2QkFBOEI7QUFFOUIseUJBQTBCO0FBRTFCLElBQUksa0JBQWtCLEdBQUcsRUFBRSxDQUFDLFlBQVksQ0FBQyxvQkFBb0IsQ0FBQyxDQUFDO0FBQy9ELElBQUksMEJBQTBCLEdBQUcsRUFBRSxDQUFDLFFBQVEsQ0FBQyw0QkFBNEIsQ0FBQyxDQUFDO0FBQzNFLElBQUksbUJBQW1CLEdBQUcsRUFBRSxDQUFDLFFBQVEsQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDO0FBQzdELElBQUksY0FBYyxHQUFHLEVBQUUsQ0FBQyxRQUFRLENBQUMsZ0JBQWdCLENBQUMsQ0FBQztBQUNuRCxJQUFJLG9CQUFvQixHQUFHLEVBQUUsQ0FBQyxZQUFZLENBQUMsc0JBQXNCLENBQUMsQ0FBQztBQUVuRSxJQUFJLGdCQUFnQixHQUFHLFNBQVMsQ0FBQztBQUVqQyxJQUFJLGdCQUFnQixHQUFHLEVBQUUsQ0FBQyxXQUFXLENBQUMsd0JBQXdCLENBQUMsQ0FBQTtBQUUvRCxJQUFJLENBQUMsY0FBYyxFQUFFO0lBQ2pCLGNBQWMsR0FBRyxFQUFFLENBQUMsS0FBSyxDQUFDLGdCQUFnQixDQUFDLENBQUM7SUFDNUMsSUFBSSxvQkFBb0IsSUFBSSxDQUFDLGNBQWMsRUFBRTtRQUN6QyxjQUFjLEdBQUcsSUFBSSxDQUFDLElBQUksQ0FBQyxnQkFBZ0IsRUFBRSxnQkFBZ0IsQ0FBQyxDQUFDO0tBQ2xFO0NBQ0o7QUFFRCxDQUFDOztRQUNHLElBQUk7WUFFQSxJQUFJLFdBQVcsR0FBaUI7Z0JBQzVCLEdBQUcsRUFBRSxTQUFTO2dCQUNkLEdBQUcsRUFBRSxTQUFTO2dCQUNkLE1BQU0sRUFBRSxTQUFTO2dCQUNqQixZQUFZLEVBQUUsU0FBUztnQkFDdkIsZ0JBQWdCLEVBQUUsU0FBUztnQkFDM0IsU0FBUyxFQUFFLFNBQVM7Z0JBQ3BCLFNBQVMsRUFBRSxTQUFTO2dCQUNwQix3QkFBd0IsRUFBRSxTQUFTO2FBQ3RDLENBQUM7WUFFRixJQUFJLFVBQXNCLENBQUM7WUFFM0IsSUFBSSxPQUFPLEdBQUcsRUFBRSxDQUFDLFFBQVEsRUFBRSxJQUFJLE9BQU8sQ0FBQztZQUV2QyxJQUFJLE9BQU8sRUFBRTtnQkFDVCxVQUFVLEdBQUcsRUFBRSxDQUFDLElBQUksQ0FBQyxjQUFjLENBQUMsQ0FBQzthQUN4QztpQkFBTTtnQkFDSCxVQUFVLEdBQUcsRUFBRSxDQUFDLElBQUksQ0FBQyxNQUFNLENBQUMsQ0FBQztnQkFDN0IsVUFBVSxDQUFDLEdBQUcsQ0FBQyxjQUFjLENBQUMsQ0FBQzthQUNsQztZQUVELFVBQVUsQ0FBQyxHQUFHLENBQUM7Z0JBQ1gsZ0JBQWdCO2dCQUNoQixTQUFTO2dCQUNULGFBQWE7Z0JBQ2IsVUFBVTthQUNiLENBQUMsQ0FBQztZQUVILElBQUksa0JBQWtCLEVBQUU7Z0JBQ3BCLFVBQVUsQ0FBQyxHQUFHLENBQUMscUJBQXFCLENBQUMsQ0FBQTtnQkFDckMsSUFBSSwwQkFBMEIsRUFBRTtvQkFDNUIsVUFBVSxDQUFDLEdBQUcsQ0FBQywwQkFBMEIsQ0FBQyxDQUFDO2lCQUM5QztxQkFBTTtvQkFDSCxVQUFVLENBQUMsR0FBRyxDQUFDLE1BQU0sQ0FBQyxDQUFDO2lCQUMxQjthQUNKO1lBRUQsSUFBSSxtQkFBbUIsRUFBRTtnQkFDckIsVUFBVSxDQUFDLElBQUksQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDO2FBQ3hDO1lBRUQsSUFBSSxNQUFNLEdBQUcsTUFBTSxVQUFVLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxDQUFDO1lBQ2hELElBQUksTUFBTSxFQUFFO2dCQUNSLEVBQUUsQ0FBQyxTQUFTLENBQUMsRUFBRSxDQUFDLFVBQVUsQ0FBQyxNQUFNLEVBQUUsOENBQThDLENBQUMsQ0FBQTthQUNyRjtpQkFBTTtnQkFDSCxFQUFFLENBQUMsU0FBUyxDQUFDLEVBQUUsQ0FBQyxVQUFVLENBQUMsU0FBUyxFQUFFLGtDQUFrQyxDQUFDLENBQUE7YUFDNUU7U0FDSjtRQUNELE9BQU8sR0FBRyxFQUFFO1lBQ1IsRUFBRSxDQUFDLEtBQUssQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLENBQUE7WUFDbkIsRUFBRSxDQUFDLFNBQVMsQ0FBQyxFQUFFLENBQUMsVUFBVSxDQUFDLE1BQU0sRUFBRSxHQUFHLENBQUMsQ0FBQztTQUMzQztJQUNMLENBQUM7Q0FBQSxDQUFDLEVBQUUsQ0FBQyJ9